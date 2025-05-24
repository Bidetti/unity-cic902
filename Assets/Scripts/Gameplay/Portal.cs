using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    public BoxCollider2D portalCollider;
    private Animator animator;

    void Start()
    {
        portalCollider = portalCollider ?? GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (portalCollider.IsTouchingLayers(LayerMask.GetMask("Player")) && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(CloseAndLoadBossFight());
        }
    }
    
    public IEnumerator CloseAndLoadBossFight()
    {
        animator.SetTrigger("close");
        yield return new WaitForSeconds(0.8f);
        Debug.Log("Iniciando batalha com o boss...");
        SceneManager.LoadScene("BossFight");
    }
}
