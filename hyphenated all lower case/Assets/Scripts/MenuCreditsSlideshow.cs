using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuCreditsSlideshow : MonoBehaviour
{
    [TextArea]
    public List<string> headers, credits;
    public TextMeshProUGUI headerText, creditsText;

    Coroutine playRoutine;

    IEnumerator CreditSlideshowC()
    {
        Color origHeaderColor = headerText.color;
        Color origCreditsColor = creditsText.color;


        //while loops zijn voor pannenkoeken
        for (int i = 0; true; i++, i%=credits.Count)
        {
            headerText.color = Color.clear;
            creditsText.color = Color.clear;
            headerText.text = headers[i];
            creditsText.text = credits[i];

            float t = 0.0f;

            //gelukkig ben ik een pannenkoek
            while (t<=1.0f)
            {
                headerText.color = Color.Lerp(Color.clear, origHeaderColor, t);
                creditsText.color = Color.Lerp(Color.clear, origCreditsColor, t); 
                
                t += Time.deltaTime;
                yield return null;
            }

            headerText.color = origHeaderColor;
            creditsText.color = origCreditsColor;
            yield return new WaitForSeconds(5.0f);

            t = 1.0f;

            //gelukkig ben ik twee pannenkoeken also hahahhahahaha code duplication, again ;_;
            while (t >= 0.0f)
            {
                headerText.color = Color.Lerp(Color.clear, origHeaderColor, t);
                creditsText.color = Color.Lerp(Color.clear, origCreditsColor, t);

                t -= Time.deltaTime;
                yield return null;
            }            
        }
    }

    private void OnEnable()
    {
        playRoutine = StartCoroutine(CreditSlideshowC());
    }

    private void OnDisable()
    {
        if(playRoutine != null) { StopCoroutine(playRoutine); }
    }


}
