﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DBCaller : MonoBehaviour
{

    public List<Utente> UtentiData { get; set; } = new List<Utente>();

    void Start()
    {
        StartCoroutine(GetUtenti());
        InsertUtente("ms53", "mas53");
    }
    public IEnumerator GetUtenti()
    {
        var UtenteData = new WWW("http://localhost:81/Infissy/UtentiData.php");
        yield return UtenteData;
        var UtenteDataString = UtenteData.text;
        var udata = UtenteDataString.Split(';');
        foreach (var u in udata)
        {

            try
            {
                var utente = new Utente();
                var usplit = u.Split('|');
                utente.IDUtente = int.Parse(usplit[0].Split(':').Last());
                utente.NomeUtente = usplit[1].Split(':').Last();
                utente.Password = usplit[2].Split(':').Last();
                UtentiData.Add(utente);
            }
            catch (Exception)
            {
                continue;
            }


        }
    }
    public void InsertUtente(string user,string passw)
    {
        var form = new WWWForm();
        form.AddField("nome", user);
        form.AddField("passw", passw);
        var www = new WWW("http://localhost:81/Infissy/UtentiInsert.php", form);
       
    }
    public class Utente
    {
        public int IDUtente { get; internal set; }
        public string NomeUtente { get; internal set; }
        public string Password { get; internal set; }
    }

}
