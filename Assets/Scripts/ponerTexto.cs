using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;

public class ponerTexto : MonoBehaviour {
	[SerializeField]
	private Text texto;
	[SerializeField]
	private Text textoInvisible;

	public int altura;
	public GameObject retroalimentador;
	public float time;

	int indice = 0;
	int indl = 0;
	ArrayList palabras = new ArrayList();
	string contenido;
	bool continuar = true;
	bool terminar = false;
	int indTemp;
	int fin;

	void Start () {
		//StreamReader txt = new StreamReader("contenido.txt", System.Text.Encoding.UTF7);
	}

	IEnumerator poniendoTexto(int inicio, int final){
		string llenar = "";
		Debug.Log (contenido.Length);
		if (final > contenido.Length) {
			final = contenido.Length;
		}
		for (int i = inicio; i < final; i++) {
			indice = i+1;
			llenar += contenido [i];
			texto.text = llenar;
			yield return new WaitForSeconds (time);
		}
		Debug.Log (contenido.Length == fin - 1);
		Debug.Log (contenido.Length);
		Debug.Log (contenido.Length);
		yield return new WaitForSeconds (1);
		terminar = true;
	}

	public void colocar(){
		if (continuar) {
			continuar = false;
			terminar = false;
			fin = palabrasMax (indl);
			StartCoroutine (poniendoTexto (indice, fin));
		}
		if (terminar && contenido.Length == fin-1) {
			retroalimentador.SetActive (false);
		}
	}

	int palabrasMax(int inicio){
		string llenar = "";
		string llenarTemp = "";
		for (int i = inicio; i < palabras.Count; i++) {
			indl = i;
			if (i != palabras.Count -1 && palabras [i + 1] != "\n") {
				llenar += palabras [i] + " ";
			} else {
				llenar += palabras [i];
			}
			textoInvisible.text = llenar;
			if (textoInvisible.preferredHeight > altura) {
				break;
			}
			if (palabras [i] != "\n") {
				
				llenarTemp += palabras [i] + " ";
			}
		}
		Debug.Log (llenarTemp.Length);
		return indice + llenarTemp.Length;
	}

	public void activar(string retroalimentacion){
		retroalimentador.SetActive (true);
		contenido = retroalimentacion;
		string[] lista = contenido.Split(' ');
		for (int i = 0; i < lista.Length; i++) {
			if (lista [i].Contains ("\n")) {
				string[] sublista = lista [i].Split ('\n');
				palabras.Add (sublista [0]);
				for (int j = 1; j < sublista.Length; j++) {
					palabras.Add ("\n");
					palabras.Add (sublista [j]);
				}
			} else {
				palabras.Add (lista [i]);
			}
		}
		colocar ();
	}
}

