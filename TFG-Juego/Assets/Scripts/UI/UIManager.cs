using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    GameObject lifeBar;
    [SerializeField]
    GameObject mierdon;
    [SerializeField]
    GameObject lifeText;
    [SerializeField]
    GameObject ammoText;
    [SerializeField]
    GameObject armas;
    float lifeScale;
    float actualLifeScale = 1;
    int lifePoints = 10;
    int actualLifePoints = 0;
    int ammo = 0;
    int max_Ammo = 0;

    [SerializeField]
    Texture2D[] imageGunsTex;
    int id = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        lifeScale = lifeBar.GetComponent<RectTransform>().localScale.y;
        setLife(GameManager.instance.getLife(), GameManager.instance.getMaxLife(), true);
        GameManager.instance.updateUIAmmos();
        changeDashInfo(GameManager.instance.getConnected());
    }

    public void reloadUI()
    {
        lifeScale = lifeBar.GetComponent<RectTransform>().localScale.y;
        setLife(GameManager.instance.getLife(), GameManager.instance.getMaxLife(), true);
        GameManager.instance.updateUIAmmos();
        changeDashInfo(GameManager.instance.getConnected());
    }
    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.E)) 
        //{
        //    setLife(1, 15, true);
        //}
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    setLife(-1, 12, false);
        //}
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    GameManager.instance.setGun(2);
        //}
    }

    public void setLife(int points, int MAX_points, bool add)
    {
        actualLifePoints = points;
        lifePoints = MAX_points;
        if(lifePoints > 0)
        {
            actualLifeScale = ((float)actualLifePoints * lifeScale / (float)lifePoints);
            lifeBar.GetComponent<RectTransform>().localScale = new Vector2(actualLifeScale, lifeScale);
            if (add)
                mierdon.GetComponent<Animator>().SetTrigger("Add");
            else
                mierdon.GetComponent<Animator>().SetTrigger("Sub");

            string points_string = actualLifePoints + "/" + lifePoints;
            lifeText.GetComponent<TextMeshProUGUI>().text = points_string;
        }

    }

    public void setAmmo(int am, int max, bool add)
    {
        if (PlayerInstance.instance.GetWeapon().infiniteAmmo)
        {
            ammoText.GetComponent<TextMeshProUGUI>().text = "-/-";
        }
        else
        {
            ammo = am;
            max_Ammo = max;
            if (ammo > 0)
            {
                if (add)
                    mierdon.GetComponent<Animator>().SetTrigger("Add");

                string ammo_string = ammo + "/" + max;
                ammoText.GetComponent<TextMeshProUGUI>().text = ammo_string;
            }
        }
    }

    public void setGun(Texture2D texture)
    {
        Sprite sprite = armas.GetComponent<Image>().sprite;
        armas.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0,0 , texture.width, texture.height), new Vector2(0.5f, 0.5f));

        if (sprite != null)
        {
            float anchoOriginal = texture.width;
            float altoOriginal = texture.height;

            armas.GetComponent<RectTransform>().sizeDelta = new Vector2(anchoOriginal*5, altoOriginal*5);
        }
    }

    public void ResetFade()
    {
        Image t = transform.GetChild(4).GetComponent<Image>();    
        t.color = new Color(t.color.r, t.color.g, t.color.b, 0);
    }

    public void DestroyThyself()
    {
        Destroy(gameObject);
        instance = null;    // because destroy doesn't happen until end of frame
    }

    public void changeDashInfo(bool con)
    {
        if (transform.GetChild(3).GetChild(0).gameObject != null)
        {
            if (con)
            {
                transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
                transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
                transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}
