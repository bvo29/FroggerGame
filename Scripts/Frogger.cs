using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frogger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deadSprite;

    private Vector3 spawnPosition;

    private float farthestRow;
    public AudioClip Jetpack;
    public AudioClip Dying;

   AudioSource audioSource;

     

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }
 private void Update()
 {
    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Move(Vector3.up);
        PlaySound(Jetpack);
        
    }
    else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Move(Vector3.down);
         PlaySound(Jetpack);
    }
    else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        Move(Vector3.left);
         PlaySound(Jetpack);
    }
    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Move(Vector3.right);
         PlaySound(Jetpack);
    }
    else if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("QUIT!");
            Application.Quit();
    }
 }

 private void Move(Vector3 direction)
 {
     Vector3 destination = transform.position + direction;

     Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));
     Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
     Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));

     if (barrier != null)
     {
        return;
     }

     if (platform != null)
     {
        transform.SetParent(platform.transform);
     }
     else
     {
        transform.SetParent(null);
     }

     if (obstacle != null && platform == null) 
     {
        transform.position = destination;
        Death();
     }
     else
     {
      if (destination.y > farthestRow)
      {
         farthestRow = destination.y;
         FindObjectOfType<GameManager>().AdvancedRow();
      }
          StartCoroutine(Leap(destination));
     }
 }

 private IEnumerator Leap(Vector3 destination)
 {
    Vector3 startPosition = transform.position;
    float elapsed = 0f;
    float duration = 0.125f;

    spriteRenderer.sprite = leapSprite;

    while (elapsed < duration)
    {
        float t = elapsed / duration;
        transform.position = Vector3.Lerp(startPosition, destination, t);
        elapsed += Time.deltaTime;
        yield return null;
    }

    transform.position = destination;

    spriteRenderer.sprite = idleSprite;
 }

 public void Death()
 {
    StopAllCoroutines();

    transform.rotation = Quaternion.identity;
    spriteRenderer.sprite = deadSprite;
    enabled = false;

    FindObjectOfType<GameManager>().Died();

    PlaySound(Dying);
 }

 public void Respawn()
 {
    StopAllCoroutines();

    transform.rotation = Quaternion.identity;
    transform.position = spawnPosition;
    farthestRow = spawnPosition.y;
    spriteRenderer.sprite = idleSprite;
    gameObject.SetActive(true);
    enabled = true;
 }

 private void OnTriggerEnter2D(Collider2D other)
 {
    if (enabled && other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && transform.parent == null)
    {
        Death();
    }
 }

   public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
