using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    private string secretKey = "mySecretKey"; // Edit this value and make sure it's the same as the one stored on the server
    const string ADD_SCORE_URL = "http://pixelreverie.net/towerdefence/addscore.php?"; //be sure to add a ? to your url
    const string GET_SCORE_URL = "http://pixelreverie.net/towerdefence/display.php";

    Action<List<Score>> OnScoreRecieve;

    [SerializeField]
    private GridLayoutGroup scoreLayoutGroup;
    [SerializeField]
    private ScoreDisplay scoreDisplayPrefab;

    private List<ScoreDisplay> activeScoreDisplays = new List<ScoreDisplay>();

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            GetScores(null);
        }
    }

    public class Score
    {
        public string name;
        public int score;

        public Score(string _name, int _score)
        {
            name = _name;
            score = _score;
        }
    }

    public void PostScore(string name, int score)
    {
        StartCoroutine(IEnum_PostScore(name, score));
    }

    public void GetScores(Action<List<Score>> onScoreReceive)
    {
        OnScoreRecieve = onScoreReceive;

        StartCoroutine(IEnum_GetScores());
    }

    IEnumerator IEnum_PostScore(string name, int score)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = Md5Sum(name + score + secretKey);

        string post_url = ADD_SCORE_URL + "name=" + WWW.EscapeURL(name) + "&score=" + score;

        Debug.Log(post_url);

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }
    }

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    IEnumerator IEnum_GetScores()
    {
        Debug.Log("Loading Scores");
        WWW hs_get = new WWW(GET_SCORE_URL);
        yield return hs_get;

        if (hs_get.error != null)
        {
            print("There was an error getting the high score: " + hs_get.error);
        }
        else
        {
            Debug.Log(hs_get.text); // this is a GUIText that will display the scores in game.

            //Snip up scores
            List<Score> fetchedScores = new List<Score>();

            string[] splitScores = hs_get.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitScores.Length; i++)
            {
                Debug.Log(splitScores[i]);

                string[] splitScore = splitScores[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                Score newScore = new Score(splitScore[0], int.Parse(splitScore[1]));
                fetchedScores.Add(newScore);
            }
            if (OnScoreRecieve != null)
            {
                OnScoreRecieve.Invoke(fetchedScores);
                OnScoreRecieve = null;
            }
        }
    }

    public void PopulateScoreTable(List<Score> scores)
    {
        ClearScoreTable();
        for (int i = 0; i < scores.Count; i++)
        {
            ScoreDisplay newScoreDisplay = Instantiate<ScoreDisplay>(scoreDisplayPrefab);
            newScoreDisplay.transform.parent = scoreLayoutGroup.transform;
            newScoreDisplay.Populate(scores[i], i+1);
            activeScoreDisplays.Add(newScoreDisplay);
        }
    }

    public void ClearScoreTable()
    {
        for (int i = activeScoreDisplays.Count - 1; i >= 0; i--)
        {
            ScoreDisplay scoreDisplay = activeScoreDisplays[i];
            activeScoreDisplays.RemoveAt(i);
            GameObject.Destroy(scoreDisplay.gameObject);
        }
    }

    public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}
