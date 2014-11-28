using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using WebApplication1.ServiceModel;

namespace WebApplication1.ServiceInterface
{
    public class BeanServices : Service
    {
        [AddHeader(ContentType = "application/x-javascript")]
        public string Any(RequestBean request)
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