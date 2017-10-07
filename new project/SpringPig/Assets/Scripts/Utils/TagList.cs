using System.Collections.Generic;
using UnityEngine;

public class TagList : MonoBehaviour {

    public List<string> _tags = new List<string>();

    public bool ContainsTag(string tag) {
        return _tags.Contains(tag);
    }

    public static bool ContainsTag(GameObject gameObject, string tag)
    {
        TagList tags = gameObject.GetComponent<TagList>();
        return tags != null && tags.ContainsTag(tag);
    }
}
