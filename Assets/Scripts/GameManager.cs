using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int token1;
    private int token2;
    private int intents;
    private float temps;
    private bool reveal = true;
    private float highscoreTemps;
    public GameState currentState;
    public enum GameState
{
    Start,          // Estado inicial cuando se arranca el juego
    Reveal,
    Reveal2,    // Cuando se están revelando cartas
    Check,     // Verificación si las cartas coinciden
    Hide,      // Ocultar las cartas si no coinciden
    EndGame         // Estado para cuando se completa el juego
}
    public GameObject startButton;
    public GameObject table;
    public GameObject[] tokens;
    public TextMeshProUGUI intentsUI;
    public TextMeshProUGUI tempsUI;
    public TextMeshProUGUI highscoreTempsUI;
    public GameObject newRecordUI;
    public AudioSource audioSource;
    public AudioClip startSound;
    public AudioClip revealSound;
    public AudioClip incorrectPairSound;
    public AudioClip correctPairSound;
    public AudioClip newRecordSound;

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.Start;
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();
    }

    void HandleState()
    {
        //Posa el temps per pantalla
        tempsUI.text = "Temps: " + temps.ToString("F2");
        switch (currentState)
        {
            case GameState.Start:
                table.SetActive(false);
                newRecordUI.SetActive(false);
                int x = 0;
                //Assigna les IDs a tots el tokens i apareixen
                foreach (GameObject token in tokens)
                {
                    token.GetComponent<Token>().SetID(x);
                    x++;
                    token.GetComponent<Token>().Appear();
                }

                intents = 0;
                intentsUI.text = "Intents: " + intents;
                highscoreTemps = PlayerPrefs.GetFloat("HighscoreTemps", 0f);
                highscoreTempsUI.text = highscoreTemps.ToString("F2") + "s";

                break;



            case GameState.Reveal:
                temps += Time.deltaTime;
                break;



            case GameState.Reveal2:
                temps += Time.deltaTime;
                break;



            case GameState.Check:
                temps += Time.deltaTime;
                //Revisa si la parella es correcte
                if (tokens[token1].GetComponent<Token>().GetPair() != tokens[token2].GetComponent<Token>().GetPair())
                {
                    currentState = GameState.Hide;
                    Invoke("IncorrectPairSound", 1.5f);
                }
                //Si es correcte fa que desapareguin i revisa si ja s'ha acabat el joc
                else 
                {
                    tokens[token1].GetComponent<Token>().SetPaired(true);
                    tokens[token2].GetComponent<Token>().SetPaired(true);
                    DissapearPair(token1, token2);
                    Invoke("CorrectPairSound", 1f);
                    CheckEndgame();
                }
                intents++;
                intentsUI.text = "Intents: " + intents;
                break;



            case GameState.Hide:
                temps += Time.deltaTime;
                HidePair(token1, token2);
                currentState = GameState.Reveal;
                Invoke("CanReveal", 2f);
                break;



            case GameState.EndGame:
                highscoreTemps = PlayerPrefs.GetFloat("HighscoreTemps", int.MaxValue);
                //Revisa si has fet un nou record
                if (temps < highscoreTemps || highscoreTemps == 0)
                {
                    //Si has fet un nou record el guarda, et felicita per pantalla i recarga l'escena als pocs segons
                    PlayerPrefs.SetFloat("HighscoreTemps", temps);
                    PlayerPrefs.Save();
                    Invoke("NewRecordSound", 3);
                    newRecordUI.SetActive(true);
                    Invoke("reloadScene", 10);
                }
                else {
                    Invoke("reloadScene", 5);
                }
                break;
            
        }
    }

    //Al donar-li a Start s'activa la taula, es donen les parelles y pasem al Reveal
    private void OnButtonClick()
    {
        startButton.SetActive(false);
        table.SetActive(true);
        AssignPairs(tokens);
        audioSource.PlayOneShot(startSound);
        currentState = GameState.Reveal;
    }

    //A partir d'una llista s'assignen les imatges dels tokens, de forma que hi hagi 8 parelles
    private void AssignPairs(GameObject[] t)
    {
        List<string> pairs = new List<string> {"Aki", "Aki", "Cammy", "Cammy", "Chunli", "Chunli", "Juri", "Juri", "Ken",
        "Ken", "Luke", "Luke", "Ryu", "Ryu", "Terry", "Terry",};

        foreach (GameObject token in t)
        {
            string randomPair = pairs[UnityEngine.Random.Range(0, pairs.Count)];
            token.GetComponent<Token>().SetPair(randomPair);
            Material newMaterial = Resources.Load<Material>("Materials/" + randomPair);
            token.GetComponent<Token>().SetImage(newMaterial);
            pairs.Remove(randomPair);
        }
    }
    
    public void Reveal(int id)
    {
        if (currentState == GameState.Reveal)
        {
            //Nomes podrem revelar un token si no esta passant res mes. Una vegada revelat es guarda l'ID del token i passem a Reveal2
            if (reveal == true)
            {
                currentState = GameState.Reveal2;
                audioSource.PlayOneShot(revealSound);
                tokens[id].GetComponent<Token>().Reveal();
                token1 = id;

            }
        }
        else if (currentState == GameState.Reveal2)
        {
            //En cas de que cliquem el mateix token que ja hem revelat no passara res.
            //En cas contrari revelarem el segon token i guardem l'ID i passem a Check
            if (id != token1)
            {
                reveal = false;
                currentState = GameState.Check;
                audioSource.PlayOneShot(revealSound);
                tokens[id].GetComponent<Token>().Reveal();
                token2 = id;
            }
        }
    }

    //Permeteix revelar un token
    private void CanReveal()
    {
        reveal = true;
    }

    //Fa el so de parella correcte
    private void CorrectPairSound()
    {
        audioSource.PlayOneShot(correctPairSound);
    }

    //Fa el so de parella incorrecte
    private void IncorrectPairSound()
    {
        audioSource.PlayOneShot(incorrectPairSound);
    }

    //Fa la animacion d'amagar als dos tokens, quan la parella sigui incorrecte
    private void HidePair(int token1, int token2)
    {
        tokens[token1].GetComponent<Token>().Invoke("Hide", 1.5f);
        tokens[token2].GetComponent<Token>().Invoke("Hide", 1.5f);
    }

    //Fa la animacio de desapareixer als dos tokens, quan la parella sigui correcte
    private void DissapearPair(int token1, int token2)
    {
        tokens[token1].GetComponent<Token>().Invoke("Disappear", 1);
        tokens[token2].GetComponent<Token>().Invoke("Disappear", 1);
    }

    //Quan fem una parella correcta revisa si hi ha mes tokens i si hi ha ens permet revelar. Si no hi ha mes tokens passem a EndGame
    private void CheckEndgame()
    {
        foreach (GameObject token in tokens)
        {
            if (token.GetComponent<Token>().GetPaired() != true)
            {
                currentState = GameState.Reveal;
                Invoke("CanReveal", 2f);
                break;
            }
            currentState = GameState.EndGame;
        }
    }

    //Fa el so de nou record
    private void NewRecordSound()
    {
        audioSource.PlayOneShot(newRecordSound);
    }

    //Recarga l'escena
    private void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
