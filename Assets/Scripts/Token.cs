using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    private GameObject gm;
    public GameObject token;
    public GameObject image;
    private Animator anim;
    private int id;
    private string pair;
    private bool paired;

    // Start is called before the first frame update
    void Start()
    {
        anim = token.GetComponent<Animator>();
        gm = GameObject.FindGameObjectWithTag("GameController");
        paired = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Si cliques un token ho reveles
    private void OnMouseDown()
    {
        if (!paired)
        {
            gm.GetComponent<GameManager>().Reveal(id);
        }
    }

    public void Reveal()
    {
        anim.ResetTrigger("Hide");
        anim.SetTrigger("Reveal");
    }

    public void Hide()
    {
        anim.ResetTrigger("Reveal");
        anim.SetTrigger("Hide");
    }

    public void Disappear()
    {
        anim.ResetTrigger("Reveal");
        anim.SetTrigger("Disappear");
    }

    public void Appear()
    {
        anim.ResetTrigger("Disappear");
        anim.SetTrigger("Appear");
    }

    public void SetID(int id)
    {
        this.id = id;
    }

    public int GetID()
    {
        return id;
    }

    public void SetPair(string pair)
    {
        this.pair = pair;
    }

    public string GetPair()
    {
        return pair;
    }

    public void SetImage(Material img)
    {
        image.GetComponent<MeshRenderer>().material = img;
    }

    public void SetPaired(bool paired)
    {
        this.paired = paired;
    }

    public bool GetPaired()
    {
        return paired;
    }
}
