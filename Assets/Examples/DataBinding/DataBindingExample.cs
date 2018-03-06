﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBindingExample : MonoBehaviour
{
    public string testStr;
    public Nest nest;

    public List<Nest> nestArray;

    [System.Serializable]
    public class Nest
    {
        public string testStr;
    }
}
