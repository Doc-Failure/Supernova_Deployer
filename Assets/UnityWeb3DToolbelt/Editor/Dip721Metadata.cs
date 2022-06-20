using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// using Unity.EditorCoroutines.Editor;

public class Dip721Metadata
{
    public string name;
    public string description;
    public Texture2D image;
    public string attributes;


    public Dip721Metadata()
    { this.attributes = "{}"; }

}