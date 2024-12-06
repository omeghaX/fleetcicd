using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PruebaFleet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PruebaFleet.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : Controller
{

    private readonly DdddbContext context;
    private readonly IConfiguration _config;
    public UserController(DdddbContext context, IConfiguration config)
    {
        this.context = context;
        _config = config;
    }

    [HttpPost]
    [Route("create")]
    //Endpoint para crear un nuevo usuario
    public async Task<ActionResult> Post(Usuario usuario)
    {

        try
        {
            if (context.Usuarios.Any(u => u.UserEmail == usuario.UserEmail))
            {
                var jOb = new
                {
                    message = HttpStatusCode.BadRequest.ToString(),
                    code = HttpStatusCode.BadRequest,
                    response = "Existe un usuario con ese correo registrado"
                };

                return BadRequest(jOb);

            }
            else
            {

                var content = usuario.UserPass.ToString();
                var key = "E546C8DF278CD5931069B522E695D4F2";

                var encrypted = EncryptString(content, key);


                var newUser = new Usuario
                {

                    UserAge = usuario.UserAge,
                    UserNombre = usuario.UserNombre,
                    UserPApellido = usuario.UserPApellido,
                    UserSApellido = usuario.UserSApellido,
                    UserEmail = usuario.UserEmail,
                    UserPass = encrypted,
                    UserGenderId = usuario.UserGenderId, //1-masculos, 2-femenino, 3-otro

                };

                context.Add(newUser);

                await context.SaveChangesAsync();

                return new JsonResult(
                       new
                       {
                           message = HttpStatusCode.OK.ToString(),
                           code = HttpStatusCode.OK,
                           response = "Usuario agregado con exito"
                       });


            }

        }
        catch (Exception ex)
        {
            var jOb = new
            {
                message = HttpStatusCode.BadRequest.ToString(),
                code = HttpStatusCode.BadRequest,
                response = ex.Message.ToString()
            };

            return BadRequest(jOb);
        }


    }



    [HttpPost]
    [Route("login")]
    //Endpoint para inicio de sesion
    public async Task<ActionResult> PostLogin(string mail, string pass)
    {
        try
        {
            var content = pass;
            var key = "E546C8DF278CD5931069B522E695D4F2";


            var userlogin = context.Usuarios.Where(s => s.UserEmail == mail).First();


            
            if (DecryptString(userlogin.UserPass.ToString(), key) == pass)
            {
                



                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                /*var claims = new[]
                {
                        new Claim(ClaimTypes.Name, userlogin.UserNombre),
                        new Claim(ClaimTypes.GivenName, userlogin.UserPApellido),
                        new Claim(ClaimTypes.Surname, userlogin.UserSApellido),
                        new Claim(ClaimTypes.Email, userlogin.UserEmail),



                };*/

                var claims = new[]
                {
                    new Claim("name", userlogin.UserNombre),
                    new Claim("givenName", userlogin.UserPApellido),
                    new Claim("surname", userlogin.UserSApellido),
                    new Claim("email", userlogin.UserEmail),
                };

                var token = new JwtSecurityToken(
                        _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                        claims,
                        expires: DateTime.Now.AddMinutes(1440),
                        signingCredentials: credentials
                    );

                    return new JsonResult(
                       new
                       {
                           message = HttpStatusCode.OK.ToString(),
                           code = HttpStatusCode.OK,
                           response = new JwtSecurityTokenHandler().WriteToken(token)
                       });
                
            }
            else
            {
                return new JsonResult(
                   new
                   {
                       message = HttpStatusCode.BadRequest.ToString(),
                       code = HttpStatusCode.BadRequest,
                       response = "mail o contraseña incorrectos"
                   });
            }
        }
        catch (Exception ex)
        {
            var jOb = new
            {
                message = HttpStatusCode.BadRequest.ToString(),
                code = HttpStatusCode.BadRequest,
                response = ex.Message.ToString()
            };

            return BadRequest(jOb);
        }
    }



    //Funcion para encriptar aes256 un string
    public static string EncryptString(string text, string keyString)
    {
        var key = Encoding.UTF8.GetBytes(keyString);

        using (var aesAlg = Aes.Create())
        {
            using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
            {
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    var iv = aesAlg.IV;

                    var decryptedContent = msEncrypt.ToArray();

                    var result = new byte[iv.Length + decryptedContent.Length];

                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }
    }

    //funcion para desencriptar string con sha256
    public static string DecryptString(string cipherText, string keyString)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        var iv = new byte[16];
        var cipher = new byte[16];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
        var key = Encoding.UTF8.GetBytes(keyString);

        using (var aesAlg = Aes.Create())
        {
            using (var decryptor = aesAlg.CreateDecryptor(key, iv))
            {
                string result;
                using (var msDecrypt = new MemoryStream(cipher))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return result;
            }
        }
    }

}

