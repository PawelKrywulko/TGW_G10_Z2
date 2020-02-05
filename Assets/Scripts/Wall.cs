using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public enum WallTags
    {
        LeftWall = 0,
        RightWall,
        TopWall,
        BottomWall
    }

    [SerializeField] public WallTags wallTag = 0;

    void Awake()
    {
        gameObject.tag = wallTag.ToString();
    }
}
