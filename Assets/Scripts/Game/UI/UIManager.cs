using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField] private TextMeshProUGUI _velocity;

    private void Start()
    {
        instance = this;
    }

    public static void SetVelocityText(string text)
    {
        instance._velocity.text = "Скорость: " + text;
    }
}
