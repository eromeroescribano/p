using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilePaths
{
    private static string HOME_DIRECTORY_SYNMBOL = "-/";
    public static string Root() { return $"{Application.dataPath}/_GameData/"; }

    //Resources Paths
    public static string Resources_graphics() { return "Graphics/"; }
    public static string Resources_backgroundImages() { return $"{Resources_graphics()}BG Images/"; }
    public static string Resources_backgroundVideos() { return $"{Resources_graphics()}BG Videos/"; }
    public static string Resources_blendTexture() { return $"{Resources_graphics()}Transition Effects/"; }

    public static string Resources_audio() { return "Audio/"; }
    public static string Resources_sfx() { return $"{Resources_audio()}SFX/"; }
    public static string Resources_voices() { return $"{Resources_audio()}Voices/"; }
    public static string Resources_music() { return $"{Resources_audio()}Music/"; }
    public static string Resources_ambience() { return $"{Resources_audio()}Ambience/"; }

    public static string Resources_dialogueFiles() { return $"Dialogue File/"; } 

    public static string GetPathToResource(string defutlPath, string resourceName)
    {
        if(resourceName.StartsWith(HOME_DIRECTORY_SYNMBOL))
        {
            return resourceName.Substring(HOME_DIRECTORY_SYNMBOL.Length);
        }
        return defutlPath + resourceName;
    }
}
