using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telepathy;

namespace TelepathyServerTest
{
    public class Startup
    {
		int s_ServerHostId = 0;

		static Type s_NetworkConnectionClass = typeof(NetworkConnection);

		static Dictionary<short, NetworkMessageDelegate> s_MessageHandlers = new Dictionary<short, NetworkMessageDelegate>();

		static Dictionary<int, NetworkConnection> s_Connections = new Dictionary<int, NetworkConnection>();

		public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

		Dictionary<short, NetworkMessageDelegate> m_MessageHandlers;

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();

			Transport.layer = new TelepathyTransport();

			Transport.layer.ServerStart("10.1.10.65", 7654, 20);

			app.Run(async context =>
			{
				
			});
		}

		private void HandleDisconnect(int connectionId, int v)
		{
			throw new NotImplementedException();
		}

		private void HandleData(int connectionId, byte[] data, int v)
		{
			throw new NotImplementedException();
		}

		private void HandleConnect(int connectionId, byte error)
		{
			System.Diagnostics.Debug.WriteLine("Server accepted client:" + connectionId);

			if (error != 0)
			{
				GenerateConnectError(error);
				return;
			}

			// get ip address from connection
			string address;
			Transport.layer.GetConnectionInfo(connectionId, out address);

			// add player info
			NetworkConnection conn = (NetworkConnection)Activator.CreateInstance(s_NetworkConnectionClass);
			conn.Initialize(address, s_ServerHostId, connectionId);
			AddConnection(conn);
			OnConnected(conn);
		}

		private void OnConnected(NetworkConnection conn)
		{
			System.Diagnostics.Debug.WriteLine("Server accepted client:" + conn.connectionId);
			conn.InvokeHandlerNoData((short)MsgType.Connect);
		}

		private static bool AddConnection(NetworkConnection conn)
		{
			if (!s_Connections.ContainsKey(conn.connectionId))
			{
				// connection cannot be null here or conn.connectionId
				// would throw NRE
				s_Connections[conn.connectionId] = conn;
				conn.SetHandlers(s_MessageHandlers);
				return true;
			}
			// already a connection with this id
			return false;
		}

		private void GenerateConnectError(byte error)
		{
			throw new NotImplementedException();
		}

		private void Disconnected(Message msg)
		{
			throw new NotImplementedException();
		}
	}
}
