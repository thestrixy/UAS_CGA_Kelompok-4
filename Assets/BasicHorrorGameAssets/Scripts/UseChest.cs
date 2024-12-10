using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseChest : MonoBehaviour
{
    private GameObject OB;
    public GameObject handUI;
    public GameObject objToActivate;
    private AudioSource audioSource;
    public AudioClip openSound;

    private Animator animator;
    private BoxCollider boxCollider;

    private bool inReach;
    private bool hasPlayedSound = false; 

    void Start()
    {
        OB = this.gameObject;

        handUI.SetActive(false);
        objToActivate.SetActive(false);

        audioSource = OB.GetComponent<AudioSource>();
        animator = OB.GetComponent<Animator>();
        boxCollider = OB.GetComponent<BoxCollider>();

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not assigned on " + OB.name);
        }

        if (openSound == null)
        {
            Debug.LogWarning("openSound AudioClip not assigned for " + OB.name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            handUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            handUI.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            handUI.SetActive(false);
            objToActivate.SetActive(true);
            animator.SetBool("open", true);
            boxCollider.enabled = false;
        }

        // Mainkan suara saat animasi peti berubah menjadi "open."
        if (animator.GetBool("open") && !hasPlayedSound)
        {
            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
                hasPlayedSound = true; 
            }
        }
    }
}
