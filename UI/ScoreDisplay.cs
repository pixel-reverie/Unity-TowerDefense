using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text numberText;

    public void Populate(ScoreboardManager.Score score, int number)
    {
        nameText.text = score.name;
        scoreText.text = score.score.ToString();
        numberText.text = string.Format("#{0}", number);
    }
}
