using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimedButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Animator animator;
    [SerializeField] private bool awakeState;

    private float startTime;
    private float duration;
    private bool enableUpdate = false;

    private void Awake()
    {
        gameObject.SetActive(awakeState);
    }

    public void Show(float duration)
    { 
        startTime = Time.time;
        this.duration = duration;
        image.fillAmount = 1;
        gameObject.SetActive(true);
        animator.SetTrigger("Show");
        enableUpdate = true;
    }

    public void Close()
    {
        enableUpdate = false;
        animator.SetTrigger("Hide");
    }

    public void SetActive(int value)
    {
        gameObject.SetActive(value != 0);
    }

    private void Update()
    {
        if (!enableUpdate) return;
        float progress = (Time.time - startTime) / duration;
        image.fillAmount = Mathf.Max(1 - progress, 0);
        if (progress >= 1)
        {
            Close();
        }
    }
}
