using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.AspNetCore;
using MQTTnet.Protocol;
using MQTTnet.Server;
using WebClient.Models;

namespace WebClient
{
    public class Startup
    {
        
        private const string AllowOrigins = "_myAllowSpecificOrigins";
        
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LampContext>(opt =>
            {
                opt.UseSqlite("Data Source=MqttSample.db;");
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            
            services.AddCors(options =>
            {
                options.AddPolicy(AllowOrigins, builder =>
                    {
                        builder.WithOrigins("ws://localhost:1884/mqtt")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                    });
            });
            //this adds a hosted mqtt server to the services
            services.AddHostedMqttServer(builder =>
            {
                builder.WithDefaultEndpointPort(1884);
                builder.WithConnectionBacklog(100);
                builder.WithConnectionValidator(ConnectionValidator);
            });
            //this adds tcp server support based on Microsoft.AspNetCore.Connections.Abstractions
            services.AddMqttConnectionHandler();
            //this adds websocket support
            services.AddMqttWebSocketServerAdapter();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvide)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            // this maps the websocket to an mqtt endpoint
            app.UseMqttEndpoint()
               .UseMqttServer(server =>
               {
                   server.ApplicationMessageReceived += ServerOnApplicationMessageReceived;
                   server.ClientConnected += (sender, e) => Debug.WriteLine($">> Disconnected {e.ClientId}");
               });
            app.UseMvc();

            GetRaspberryResult(serviceProvide).GetAwaiter();
        }

        private async Task GetRaspberryResult(IServiceProvider serviceProvide)
        {
            var server = serviceProvide.GetService<IMqttServer>();
            await server.SubscribeAsync(
                       "message-brocker", 
                       new List<TopicFilter>()
                       {
                           new TopicFilterBuilder().WithTopic("pi-result").Build()
                       });
        }

        private static void ServerOnApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine("\nThere is a fucking message comes around!!.");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {payload}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine($"+ ClientId = {e.ClientId}");

            if (!e.ApplicationMessage.Topic.Equals("pi-result")) return;
            var optionsBuilder = new DbContextOptionsBuilder<LampContext>();
            optionsBuilder.UseSqlite("Data Source=MqttSample.db;");
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            using (var db = new LampContext(optionsBuilder.Options))
            {
                db.Lamp.Add(new LampSourcing() { Status = payload });
                db.SaveChanges();
            }
        }

        private static void ConnectionValidator(MqttConnectionValidatorContext c)
        {
            if (c.ClientId.Length < 10)
            {

            }
//            if (!c.Username.Equals("pi"))
//            {
//                c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
//                return;
//            }
//            if (!c.Password.Equals("mj06174551gh"))
//            {
//                c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
//                return;
//            }
            c.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
        }

    }
}