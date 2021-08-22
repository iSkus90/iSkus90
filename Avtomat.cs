using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
class UserInfo
{
    // Метод, реализующий словарь
    public static Dictionary<string, int> MyDic()
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();

        dic.Add("cupcakes", 10 );
        dic.Add("blueberry", 7 );
        dic.Add("bananas", 15);
        dic.Add("honey", 14);
        dic.Add("cake2", 13);
        dic.Add("iceCream", 1);
        dic.Add("", 1000);
        dic.Add("avocado", 5);
        dic.Add("apple", 4);
        //dic.Add("Human", 0);

        return dic;
    }
}


public class Avtomat : MonoBehaviour
{
    static private Avtomat _instance;
    public static Avtomat Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    [SerializeField] public string[,] saveNameProdukti = new string[,] { 
        { "f", "h", "h"},  //00 01 02 03 04
        { "c", "i", "h" }, //10 11 12 13 14
        { "d", "z", "h" } };
    private string[] nameProdukti = new string[] { "cupcakes", "blueberry", "honey", "cake2", "bananas", "iceCream", "avocado", "apple" }; //записать сюда все возможные названия

    public GameObject ButtonPrefab;
    public ScrollRect _skroll;
    private int _coin = 20;
    private int rHuman; //Столбец Human
    private int _sHuman = 0;//Строка Human
    public GameObject _PanelWin;
    public GameObject _PanelLose;
    private int _StrokaRazmer = 3;
    private int _StolbecRazmer = 3;
        

    private void Start()
    {
        CreateButtonStart();

        HUD.Instance.coinChange(_coin);
    }

    // Рандомное заполнение элементов (создаются кнопки автомата, заполняются фруктами и хуман)
    private void CreateButtonStart()
    {
        //рандомно перемешивает массив
        for (int i = nameProdukti.Length - 1; i >= 1; i--)
        {
            int j = Random.Range(0, nameProdukti.Length);
            // обменять значения data[j] и data[i]
            var temp = nameProdukti[j];
            nameProdukti[j] = nameProdukti[i];
            nameProdukti[i] = temp;
        }

        int n = 0;

        //делаем человечка и 1 строку кнопок
        rHuman = Random.Range(0, _StolbecRazmer);     // Столбец для Human
        for (int i = 0; i < _StolbecRazmer; i++)
        {
            if (i == rHuman)
            {
                GameObject clone = Instantiate(ButtonPrefab, transform.position, transform.rotation);
                clone.transform.SetParent(_skroll.content, false);

                saveNameProdukti[0, i] = "Human";
                clone.name = "Human";
                clone.SetActive(true);
            }

            else
            {
                GameObject clone = Instantiate(ButtonPrefab, transform.position, transform.rotation);
                clone.transform.SetParent(_skroll.content, false);

                saveNameProdukti[0, i] = nameProdukti[n];
                n++;
                clone.name = saveNameProdukti[0, i];
                clone.SetActive(true);
            }
        }
        

        //создает кнопки автомата из перемешанного массива

        for (int i = 1; i < _StrokaRazmer; i++)
        {
            for (int j = 0; j < _StolbecRazmer; j++)
            {
                GameObject clone = Instantiate(ButtonPrefab, transform.position, transform.rotation);
                clone.transform.SetParent(_skroll.content, false);

                saveNameProdukti[i, j] = nameProdukti[n];
                n++;
                clone.name = saveNameProdukti[i, j];

                clone.SetActive(true);
            }
        }
    }
    public int Cost(string s)
    {
        Dictionary<string, int> dic = UserInfo.MyDic();
        //Debug.Log(dic[s]);
        return dic[s];
    }    

    public void Buy(string s)
    {        
        if (_coin >= Cost(s))
        {            
            _coin -= Cost(s);
            HUD.Instance.coinChange(_coin);
            for (int i = 0; i < _StrokaRazmer; i++) //i=Строка
            {
                for (int j = 0; j < _StolbecRazmer; j++) //j=Столбец
                {                    
                    if(s == saveNameProdukti[i, j])
                    {                        
                        NearbyHuman(i, j);
                    }       
                }
            }

            //проверка на победу
            if (_sHuman == _StrokaRazmer -1)
            {
                HUD.Instance.ShowWinPanel(_PanelWin);
                Debug.Log("Победили");
            }

            //проверка на поражение (закончились деньги)
            if (rHuman != 0 && rHuman != _StolbecRazmer - 1)
            {
                //проверка низ, право, лево (если не крайние)
                if (_coin < Cost(saveNameProdukti[_sHuman + 1, rHuman]) && _coin < Cost(saveNameProdukti[_sHuman, rHuman - 1]) && _coin < Cost(saveNameProdukti[_sHuman, rHuman + 1]))
                {
                    HUD.Instance.ShowWinPanel(_PanelLose);
                    Debug.Log("Проиграл");
                }
            }
            else if (rHuman == 0)
            {
                //проверка низ, право (если не крайний левый)
                if (_coin < Cost(saveNameProdukti[_sHuman + 1, rHuman]) && _coin < Cost(saveNameProdukti[_sHuman, rHuman + 1]))
                {
                    HUD.Instance.ShowWinPanel(_PanelLose);
                    Debug.Log("Проиграл");
                }
            }
            else if (rHuman == _StolbecRazmer - 1)
            {
                //проверка низ, лево (если не крайний правый)
                if (_coin < Cost(saveNameProdukti[_sHuman + 1, rHuman]) && _coin < Cost(saveNameProdukti[_sHuman, rHuman - 1]))
                {
                    HUD.Instance.ShowWinPanel(_PanelLose);
                    Debug.Log("Проиграл");
                }
            }

        }        
    }

   private void NearbyHuman(int stroka, int stolbec) //Проверка, рядом ли с Human купленный обьект
    {        
        if (stroka == _sHuman && (stolbec == rHuman+1 || stolbec == rHuman - 1))
        {
            Debug.Log("Рядом строчка: ");
            MoveHuman(stroka, stolbec);
        }
        else if(stolbec == rHuman && stroka == _sHuman+1)
        {
            Debug.Log("Рядом столбец: ");
            MoveHuman(stroka, stolbec);
        }
        
    }
   
    private void MoveHuman(int stroka, int stolbec) //Перемещение Human  (сдесь закончили)
    {    

        Produkti produkti = GameObject.Find(saveNameProdukti[stroka, stolbec]).GetComponent<Produkti>();
        produkti.ShowImage("Human");        
        produkti.ShowText("---");        
        
        produkti = GameObject.Find("Human").GetComponent<Produkti>();
        produkti.ShowImage("water");
        GameObject.Find("Human").name = "";

        GameObject.Find(saveNameProdukti[stroka, stolbec]).name = "Human";
        saveNameProdukti[stroka, stolbec] = "Human";

        saveNameProdukti[stroka, stolbec] = "";
        _sHuman = stroka;
        rHuman = stolbec;     
        

    }
}
