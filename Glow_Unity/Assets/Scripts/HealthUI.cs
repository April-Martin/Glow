using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

    public PlayerController player;
    public Image fullIcon;
    public Image emptyIcon;

    private int maxHealth;
    private Image[] icons;
    private bool[] iconStati;
    private float iconWidth;

	// Use this for initialization
	void Start () {
        iconWidth = emptyIcon.rectTransform.rect.width;
        maxHealth = player.maxHealth;
        icons = new Image[maxHealth];
        iconStati = new bool[maxHealth];

        for (int i=0; i<maxHealth; i++)
        {
            icons[i] = (UnityEngine.UI.Image)Instantiate(fullIcon);
            icons[i].transform.SetParent(transform, false);
            icons[i].rectTransform.anchoredPosition += new Vector2 ((iconWidth+2) * i, 0);
            iconStati[i] = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetHealthTo(int newHealth)
    {
        for (int iFull = 0; iFull < newHealth; iFull++ )
        {
            if (!iconStati[iFull])
            {
                Destroy(icons[iFull].gameObject);
                icons[iFull] = (UnityEngine.UI.Image)Instantiate(fullIcon);
                icons[iFull].transform.SetParent(transform, false);
                icons[iFull].rectTransform.anchoredPosition += new Vector2((iconWidth + 2) * iFull, 0);
                iconStati[iFull] = true;
            }
        }
        for (int iEmpty = newHealth; iEmpty < maxHealth; iEmpty++)
        {
            if (iconStati[iEmpty])
            {
                Destroy(icons[iEmpty].gameObject);
                icons[iEmpty] = (UnityEngine.UI.Image)Instantiate(emptyIcon);
                icons[iEmpty].transform.SetParent(transform, false);
                icons[iEmpty].rectTransform.anchoredPosition += new Vector2((iconWidth + 2) * iEmpty, 0);
                iconStati[iEmpty] = false;
            }
        }

    }
}
