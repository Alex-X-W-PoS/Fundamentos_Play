using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData{
	public int userId;
	public string userHash;
	public string nombre;
	public string username;
	public string contrasena;
	public int maximo_nivel;
	public int ultimo_nivel_visitado;
	public int preguntas_totales;
	public int preguntas_correctas;
	public int preguntas_erroneas;
	public int avatar;
	public MedalData[] medal_data;

	public UserData(){
		medal_data = new MedalData[9];
	}


	public bool[] getMedalDataUnit(int i){

		return new bool[]{medal_data [i].medalla_principiante, medal_data [i].medalla_intermedio, medal_data [i].medalla_experto};
	}

	public MedalData getMedalDataForUnit(int i){

		return medal_data[i];
	}

	public MedalData[] getMedalData(){
		return medal_data;
	}
}
