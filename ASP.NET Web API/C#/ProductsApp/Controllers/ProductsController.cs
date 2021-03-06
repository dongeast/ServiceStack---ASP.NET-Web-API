﻿using ProductsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using ServiceStack;

namespace ProductsApp.Controllers
{
    public class BeansController : ApiController
    {
        public object GetAllProducts()
        {
            List<Bean> beans = new List<Bean>();
            beans.Add(new Bean("com.unityconceptapps.tcr.kiat", 1, ""));
            beans.Add(new Bean("com.playgame.good.tankwars3D", 1, ""));
            beans.Add(new Bean("com.estoty.game2048", 1, ""));

            Response response = new Response { Result = 1, Message = beans.ToJson() };
            return response.ToJson();
        }
    }
}
