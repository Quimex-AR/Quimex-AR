using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirebaseLoginService
{
  private readonly FirebaseAuth auth;
  private readonly FirebaseFirestore firestore;

  public FirebaseLoginService()
  {
    auth = FirebaseAuth.DefaultInstance;
    firestore = FirebaseFirestore.DefaultInstance;
  }


  public async Task<(bool success, string message)> AuthenticateUserAsync(string email, string password)
  {
    if (!FirebaseInitializator.IsFirebaseReady)
      return (false, "Firebase no está listo. Intenta más tarde.");

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
      return (false, "Correo o contraseña vacíos.");

    bool userExists = await CheckUserExistsAsync(email);

    if (userExists)
    {
      return await SignInAsync(email, password);
    }
    else
    {
      return await SignUpAsync(email, password);
    }
  }

  public async Task<bool> CheckUserExistsAsync(string email)
  {
    try
    {
      var userCollection = firestore.Collection("Users");
      var query = userCollection.WhereEqualTo("email", email);
      var querySnapshot = await query.GetSnapshotAsync();

      return querySnapshot.Count > 0;
    }
    catch (Exception ex)
    {
      Debug.LogError($"Error checking user existence: {ex.Message}");

      return false;
    }
  }

  public async Task<(bool success, string message)> SignInAsync(string email, string password)
  {
    if (!FirebaseInitializator.IsFirebaseReady)
      return (false, "El servicio aun no está listo. Intenta más tarde.");

    Credential credential = EmailAuthProvider.GetCredential(email, password);

    return await auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(async task =>
    {
      if (task.IsCanceled)
        return (false, "Login Cancelado");
      if (task.IsFaulted)
        return (false, $"Error en login: {task.Exception?.GetBaseException().Message}");

      AuthResult result = task.Result;
      await SaveUserToFirestore(result.User);
      return (true, $"Login exitoso: {result.User.Email}");
    }).Unwrap();
  }

  public async Task<(bool sucess, string message)> SignUpAsync(string email, string password)
  {
    if (!FirebaseInitializator.IsFirebaseReady)
      return (false, "El servicio aun no está listo. Intenta más tarde.");

    return await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(async task =>
    {
      if (task.IsCanceled)
        return (false, "Registro cancelado");
      if (task.IsFaulted)
        return (false, $"Error en registro: {task.Exception?.GetBaseException().Message}");

      AuthResult result = task.Result;
      await SaveUserToFirestore(result.User);
      return (true, $"Usuario creado exitosamente: {result.User.Email}");
    }).Unwrap();
  }
  private async Task SaveUserToFirestore(FirebaseUser user)
  {
    try
    {
      var userDoc = firestore.Collection("Users").Document(user.UserId);
      var userData = new Dictionary<string, object>
      {
        { "userId", user.UserId },
        { "email", user.Email },
        { "lastLogin", Timestamp.GetCurrentTimestamp() }
      };

      await userDoc.SetAsync(userData, SetOptions.MergeAll);
      Debug.Log($"User data saved/updated for: {user.Email}");
    }
    catch (Exception ex)
    {
      Debug.Log($"Error saving user to Firestore: {ex.Message}");
    }
  }
}
