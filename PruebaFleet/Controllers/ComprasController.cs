using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaFleet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace PruebaFleet.Controllers;

[ApiController]
[Route("api/compras")]
public class ComprasController : Controller
{
    private readonly DdddbContext context;
    private readonly IConfiguration _config;
    public ComprasController(DdddbContext context, IConfiguration config)
    {
        this.context = context;
        _config = config;
    }




    [HttpGet]
    [Authorize]
    public async Task<ActionResult> getAll()
    {

        try
        {
            var compras = await (from compra in context.Compras
                                 join usuario in context.Usuarios
                                 on compra.CompUsuarioGuid equals usuario.UserGuid
                                 where compra.CompStatus == 1
                                 select new CompraDto
                                 {
                                     Id = compra.CompId,
                                     Categoria = compra.CompCategoria,
                                     TipoGarantia = compra.CompTipoGarantia,
                                     Usuario = usuario.UserNombre +" "+ usuario.UserPApellido +" "+ usuario.UserSApellido,
                                     Fecha = compra.CompDCreate.Date // Seteamos el datetime a solo date sin hora
                                 }).ToListAsync();



            return new JsonResult(
                   new
                   {
                       message = HttpStatusCode.OK.ToString(),
                       code = HttpStatusCode.OK,
                       response = compras
                   });
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
    [Authorize]
    public async Task<ActionResult> create(Compra compra )
    {

        try
        {

            String garantia = "";

            if (compra.CompCategoria.ToLower() == "electronico")
            {
                garantia = "1 mes";
            }
            else if (compra.CompCategoria.ToLower() == "linea blanca")
            {
                garantia = "2 meses";
            }
            else
            {
                garantia = "3 meses";
            }

            //Obtenemos los claims del JWT
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            //Obtenemos el email del jwt que ejecuta el endpoint
            string usermail = claims.Where(
                    s => s.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
                    ).First().Value.ToString();
            //Obtenemos el usuario del corre del jwt para almacenarlo en la compra creada
            var user = context.Usuarios.Where(
                u => u.UserEmail == usermail
                    ).First();


            var comp = new Compra
            {

                CompCategoria = compra.CompCategoria,
                CompProducto = compra.CompProducto,
                CompTipoGarantia = garantia,
                CompUsuarioGuid = user.UserGuid,

            };

            context.Add(comp);

            await context.SaveChangesAsync();


            return new JsonResult(
                   new
                   {
                       message = HttpStatusCode.OK.ToString(),
                       code = HttpStatusCode.OK,
                       response = "Compra realizada exitosamente"
                   });
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



    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> delete(Guid id)
    {

        try
        {

            if(context.Compras.Any(s=> s.CompId == id && s.CompStatus==1))
            {

                var compra = context.Compras.FirstOrDefault(s=> s.CompId==id);

                compra.CompStatus = 0;

                context.Update(compra);

                await context.SaveChangesAsync();

                var jOb = new
                {
                    message = HttpStatusCode.OK.ToString(),
                    code = HttpStatusCode.OK,
                    response = "Compra eliminada correctamente"
                };

                return BadRequest(jOb);
            }
            else
            {
                var jOb = new
                {
                    message = HttpStatusCode.NotFound.ToString(),
                    code = HttpStatusCode.NotFound,
                    response = "La compra no existe"
                };

                return BadRequest(jOb);

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


    [HttpPut]
    [Authorize]
    public async Task<ActionResult> update(Compra compra)
    {

        try
        {

            if (context.Compras.Any(s => s.CompId == compra.CompId && s.CompStatus == 1))
            {

                var objCompra = context.Compras.FirstOrDefault(s => s.CompId == compra.CompId);

                objCompra.CompCategoria = compra.CompCategoria;
                objCompra.CompProducto = compra.CompProducto;

                String garantia = "";

                if (compra.CompCategoria.ToLower() == "electronico")
                {
                    garantia = "1 mes";
                }
                else if (compra.CompCategoria.ToLower() == "linea blanca")
                {
                    garantia = "2 meses";
                }
                else
                {
                    garantia = "3 meses";
                }

                objCompra.CompTipoGarantia = garantia;

                context.Update(objCompra);

                await context.SaveChangesAsync();

                var jOb = new
                {
                    message = HttpStatusCode.OK.ToString(),
                    code = HttpStatusCode.OK,
                    response = "Compra actualizada correctamente"
                };

                return BadRequest(jOb);
            }
            else
            {
                var jOb = new
                {
                    message = HttpStatusCode.NotFound.ToString(),
                    code = HttpStatusCode.NotFound,
                    response = "La compra no existe o esta descuntinuada"
                };

                return BadRequest(jOb);

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

    public class CompraDto //Clase respuesta seteada con los detalles de la compra
    {
        public Guid Id { get; set; }
        public string Categoria { get; set; } = null!;
        public string TipoGarantia { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public DateTime Fecha { get; set; }
    }

}

