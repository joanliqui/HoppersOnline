using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomPassword 
{
   public static string GenerateRandomPassword(int longitud)
    {
        System.Random rdn = new System.Random();
        string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int l = caracteres.Length;
        char letter;
        string password = string.Empty;

        for (int i = 0; i < longitud; i++)
        {
            letter = caracteres[rdn.Next(longitud)];
            password += letter.ToString();
        }

        return password;
    }
}
