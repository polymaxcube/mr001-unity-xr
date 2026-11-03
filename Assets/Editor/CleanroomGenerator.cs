using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class CleanroomGenerator
{
    const string ScenePath = "Assets/Scenes/Cleanroom.unity";
    const string MaterialFolder = "Assets/Materials/CleanroomGenerated";
    const string RequestPath = "Temp/CleanroomGenerate.request";
    const string ResultPath = "Temp/CleanroomGenerate.result.json";

    [InitializeOnLoadMethod]
    static void OnLoad()
    {
        if (File.Exists(RequestPath)) EditorApplication.delayCall += RunRequested;
    }

    static void RunRequested()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += RunRequested;
            return;
        }
        if (!File.Exists(RequestPath)) return;
        File.Delete(RequestPath);
        try { Generate(); }
        catch (Exception e)
        {
            File.WriteAllText(ResultPath, JsonUtility.ToJson(new Result { success = false, error = e.ToString() }));
            Debug.LogException(e);
        }
    }

    [Serializable]
    class Result
    {
        public bool success;
        public string scene;
        public int objects;
        public string error;
    }

    [MenuItem("Tools/Cleanroom/Generate")]
    public static void Generate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Exit Play Mode before generating.");
        // Never replace a loaded scene: it may contain unsaved user changes.
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).path == ScenePath)
                throw new InvalidOperationException("Close Cleanroom.unity before regenerating it.");

        Directory.CreateDirectory("Assets/Scenes");
        Directory.CreateDirectory(MaterialFolder);
        AssetDatabase.Refresh();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) throw new InvalidOperationException("No basic Lit shader available.");
        Material white = MaterialAsset("White", Color.white, shader);
        Material floor = MaterialAsset("Floor", new Color(.72f, .74f, .76f), shader);
        Material metal = MaterialAsset("Metal", new Color(.55f, .58f, .61f), shader);
        Material light = MaterialAsset("Light", Color.white, shader);
        light.EnableKeyword("_EMISSION");
        light.SetColor("_EmissionColor", Color.white * 2f);
        Material glass = MaterialAsset("Glass", new Color(.72f, .9f, .96f, .22f), shader);
        if (glass.HasProperty("_Surface"))
        {
            glass.SetFloat("_Surface", 1);
            glass.SetFloat("_Blend", 0);
        }
        else
        {
            glass.SetFloat("_Mode", 3);
            glass.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        glass.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        glass.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        glass.SetInt("_ZWrite", 0);
        glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        glass.SetOverrideTag("RenderType", "Transparent");
        glass.renderQueue = (int)RenderQueue.Transparent;
        foreach (var m in new[] { white, floor, metal, light, glass }) EditorUtility.SetDirty(m);

        Scene previous = SceneManager.GetActiveScene();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        try
        {
            SceneManager.SetActiveScene(scene);
            var root = new GameObject("Cleanroom");
            // Clear internal dimensions: x=4m, z=5m, y=3m.
            Box(root, "Floor", new Vector3(0,-.1f,0), new Vector3(4.2f,.2f,5.2f), floor);
            Box(root, "Ceiling", new Vector3(0,3.1f,0), new Vector3(4.2f,.2f,5.2f), white);
            Box(root, "Wall_Back", new Vector3(0,1.5f,2.6f), new Vector3(4.2f,3,.2f), white);
            Box(root, "Wall_Left", new Vector3(-2.1f,1.5f,0), new Vector3(.2f,3,5), white);
            // Door aperture: width 1m, height 2.2m in front wall.
            Box(root, "Wall_Front_Left", new Vector3(-1.3f,1.5f,-2.6f), new Vector3(1.6f,3,.2f), white);
            Box(root, "Wall_Front_Right", new Vector3(1.3f,1.5f,-2.6f), new Vector3(1.6f,3,.2f), white);
            Box(root, "Wall_Front_Header", new Vector3(0,2.6f,-2.6f), new Vector3(1,.8f,.2f), white);
            Box(root, "Entrance_Door", new Vector3(0,1.1f,-2.6f), new Vector3(.96f,2.16f,.06f), white);
            // Window aperture in right wall: 1.6m wide, 1.2m high.
            Box(root, "Wall_Right_Front", new Vector3(2.1f,1.5f,-1.65f), new Vector3(.2f,3,1.7f), white);
            Box(root, "Wall_Right_Back", new Vector3(2.1f,1.5f,1.65f), new Vector3(.2f,3,1.7f), white);
            Box(root, "Wall_Right_Sill", new Vector3(2.1f,.55f,0), new Vector3(.2f,1.1f,1.6f), white);
            Box(root, "Wall_Right_Header", new Vector3(2.1f,2.65f,0), new Vector3(.2f,.7f,1.6f), white);
            Box(root, "Glass_Window", new Vector3(2.1f,1.7f,0), new Vector3(.025f,1.2f,1.6f), glass);
            for (int i = 0; i < 2; i++)
            {
                var panel = Box(root, "Ceiling_Light_" + (i+1), new Vector3(0,2.97f,i==0?-1.5f:1.5f), new Vector3(1.2f,.06f,.45f), light);
                var source = panel.AddComponent<Light>();
                source.type = LightType.Point;
                source.color = Color.white;
                source.intensity = 2f;
                source.range = 7f;
                source.shadows = LightShadows.None;
            }
            Box(root, "Ceiling_FFU", new Vector3(0,2.9f,0), new Vector3(1.2f,.2f,.6f), metal);
            Box(root, "FFU_Filter", new Vector3(0,2.79f,0), new Vector3(1.08f,.02f,.48f), white);
            int count = root.GetComponentsInChildren<Transform>(true).Length;
            if (scene.rootCount != 1 || count != 18)
                throw new InvalidOperationException("Unexpected hierarchy/object count: " + count);
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new IOException("Could not save " + ScenePath);
            AssetDatabase.SaveAssets();
            File.WriteAllText(ResultPath, JsonUtility.ToJson(new Result { success = true, scene = ScenePath, objects = count, error = "" }));
            Debug.Log("CLEANROOM_OK scene=" + ScenePath + " objects=" + count);
        }
        finally
        {
            if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
            if (scene.IsValid() && scene.isLoaded) EditorSceneManager.CloseScene(scene, true);
        }
    }

    static Material MaterialAsset(string name, Color color, Shader shader)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }
        material.color = color;
        material.SetFloat("_Smoothness", name == "Glass" ? .85f : .25f);
        return material;
    }

    static GameObject Box(GameObject root, string name, Vector3 position, Vector3 size, Material material)
    {
        var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(root.transform, false);
        box.transform.localPosition = position;
        box.transform.localScale = size;
        box.GetComponent<Renderer>().sharedMaterial = material;
        return box;
    }
}
