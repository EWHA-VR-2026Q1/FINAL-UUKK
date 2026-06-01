using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiderInteraction : MonoBehaviour
{
    public float interactDistance = 2.0f;
    public string nextSceneName = "Scene6_scaryspider";

    private Transform player;

    void Start()
    {
        player = FindObjectOfType<OVRCameraRig>().transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
