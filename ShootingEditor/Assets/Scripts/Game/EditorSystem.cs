using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Demo;
using Game;

public class EditorSystem : MonoBehaviour {

    Boss boss;
    public void setBoss(Boss boss)
    {
        this.boss = boss;
    }

    public Dropdown dropName;
    public Transform root;
    public GameObject pfItem;
    public List<ShotName> names;
    public List<GameObject> objNames;

    private void Start()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        dropName.ClearOptions();
        for (int i = 0; i < 17; i++)
        {
            ShotName _name = (ShotName)i;
            options.Add(new Dropdown.OptionData(_name.ToString()));
        }
        dropName.AddOptions(options);
        names = new List<ShotName>();
        objNames = new List<GameObject>();
    }
    public void Add()
    {
        ShotName _name = (ShotName)dropName.value;
        names.Add(_name);
        GameObject _obj = Instantiate(pfItem, root);
        objNames.Add(_obj);
        _obj.SetActive(true);
        Text _txt = _obj.GetComponentInChildren<Text>();
        _txt.text = _name.ToString();
    }

    public void Remove() {
        if (names.Count > 0)
        {
            names.RemoveAt(names.Count - 1);
            GameObject obj = objNames[objNames.Count - 1];
            objNames.RemoveAt(objNames.Count - 1);
            Destroy(obj);
        }
    }
    public void Apply()
    {
        if (names.Count ==0)
        {
            ShotName _name = (ShotName)dropName.value;
            names.Add(_name);
        }
        for (int i = 0; i < objNames.Count; i++)
        {
            Destroy(objNames[i]);
        }
        objNames = new List<GameObject>();
        //
        boss.AddShotBuilder(names);
        names = new List<ShotName>();
    }
}
