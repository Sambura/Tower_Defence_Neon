using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this script on any gameObject and assign spriteRenderer and script will randomly put there sprite from list based on probabilty
/// </summary>
public class RandomSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<SpriteProbability> sprites;
    [SerializeField] private bool ignoreProbability = false;
    [SerializeField] private bool playOnAwake = true;

    private float fullProbability = 0;

    /// <summary>
    /// Assigns random sprite to the spriteRenderer from the list according to probability and settings
    /// </summary>
    public void PutRandomSprite()
    {
        if (ignoreProbability)
        {
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)].sprite;
        } else
        {
            float value = Random.Range(0, fullProbability);
            foreach (var i in sprites)
            {
                value -= i.probability;
                if (value <= 0)
                {
                    spriteRenderer.sprite = i.sprite;
                    break;
                } 
            }
        }
    }

    private void Awake()
    {
        if (!ignoreProbability)
        {
            sprites.Sort();
            foreach (var i in sprites)
                fullProbability += i.probability;
        }
        if (playOnAwake) PutRandomSprite();
    }

    [System.Serializable]
    private class SpriteProbability : System.IComparable<SpriteProbability>
    {
        public Sprite sprite;
        public float probability;

        public int CompareTo(SpriteProbability other)
        {
            return probability.CompareTo(other.probability);
        }
    }
}
