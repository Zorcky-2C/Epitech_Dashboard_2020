using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Dashboard.Data;
using Dashboard.Infrastructure;
using Dashboard.Infrastructure.ApplicationUserClaims;
using Dashboard.Infrastructure.AppSettingsModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dashboard.Infrastructure.Startup;
using Dashboard.Models.Identity;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Linq;

namespace Dashboard
{
    public class Startup
    {

        public static List<Service> Services = new List<Service>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed
                // for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // !! Note:
                //
                // If you switch database providers, you might be required to re-create the migrations
                // as they are not always compatible between database systems

                // The easiest option for development outside a container is to use SQLite
                // options.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));
                // Or use this for PostgreSQL:
                options.UseNpgsql(Configuration.GetConnectionString("PostgresConnection"));

                // Use this to connect to a MySQL server:
                // options.UseMySQL(Configuration.GetConnectionString("MysqlConnection"));
                // Or use this for SQL Server (if running on Windows):
                // options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection"));
            });

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(options =>
            {
                //options.DefaultChallengeScheme = "GitHub";
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = "1bf4cef0-30a0-4c42-8984-11e0c0d74657";
                microsoftOptions.ClientSecret = "FZakc47uMJ--S182_840rG683A5.gM1tDs";
            });

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Default User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "Dashboard.AppCookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                // You might want to only set the application cookies over a secure connection:
                // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });

            // As per https://github.com/aspnet/AspNetCore/issues/5828
            // the settings for the cookie would get overwritten if using the default UI so
            // we need to "post-configure" the authentication cookie
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
            {
                options.AccessDeniedPath = "/access-denied";
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";

                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });

            services.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>();

            services.AddAntiforgery();

            services.Configure<ScriptTags>(Configuration.GetSection(nameof(ScriptTags)));

            services.AddControllersWithViews(options =>
            { 
                // Slugify routes so that we can use /employee/employee-details/1 instead of
                // the default /Employee/EmployeeDetails/1
                //
                // Using an outbound parameter transformer is a better choice as it also allows
                // the creation of correct routes using view helpers
                options.Conventions.Add(
                    new RouteTokenTransformerConvention(
                        new SlugifyParameterTransformer()));

                // Enable Antiforgery feature by default on all controller actions
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddRazorPages(options =>
            {
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "/register");
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "/login");
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Logout", "/logout");
                options.Conventions.AddAreaPageRoute("Identity", "/Account/ForgotPassword", "/forgot-password");
                options.Conventions.AddAreaPageRoute("Identity", "/Account/ExternalLogin", "/external-login");
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddSessionStateTempDataProvider();

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // You probably want to use in-memory cache if not developing using docker-compose
            // services.AddMemoryCache();
            services.AddDistributedRedisCache(action => { action.Configuration = Configuration["Redis:InstanceName"]; });

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.Name = "Dashboard.SessionCookie";
                // You might want to only set the application cookies over a secure connection:
                // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            // This adds a hosted service that, on application start-up, seeds the database with initial data.
            // You can remove this if you want to prevent the seeding process or you can change the initial data
            // to suit your needs in the IdentityDataSeeder class.
            services.AddHostedService<DbSeederHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // This is required to make the application work behind a proxy like NGINX or HAPROXY
            // that also provides TLS termination (switching from incoming HTTPS to HTTP)
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/status-code", "?code={0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(eb =>
            {
                eb.MapRazorPages();
                eb.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");
            });

            AddService("Epitech Intranet");
            AddService("Github");
            AddService("IP infos");
            AddService("CatAPI");

            AddWidget("Epitech Intranet", "Notes");
            AddWidget("Epitech Intranet", "Notifications");
            AddWidget("IP infos", "Show my infos");
            AddWidget("Github", "Notifications");
            AddWidget("CatAPI", "Cat");
        }

        public void AddService(string servicename)
        {
            if (!ContainsService(servicename))
            {
                Service service = new Service(servicename);
                Services.Add(service);
            }
        }

        public void AddWidget(string servicename, string widgetname)
        {
            if (widgetname == "Notes")
            {
                Widget notes = new Widget();
                notes.Size = 1;
                notes.Body = "Widgets/Intranet/_Notes";
                notes.Name = "Notes";
                notes.Description = "Show your credits and GPA";
                notes.Id = "000";

                GetService(servicename).Widgets.Add(notes);
            }
            else if (servicename == "Epitech Intranet" && widgetname == "Notifications")
            {
                Widget notifications = new Widget();
                notifications.Size = 2;
                notifications.Body = "Widgets/Intranet/_Notifications";
                notifications.Name = "Notifications";

                notifications.Description = "Show your latest Intranet notifications";
                notifications.Id = "001";

                GetService(servicename).Widgets.Add(notifications);
            }
            else if (widgetname == "Show my infos")
            {
                Widget showip = new Widget();
                showip.Size = 1;
                showip.Body = "Widgets/Ip/_Ip";
                showip.Name = "Show my infos";
                showip.Description = "Show your public IP address and more informations about it";
                showip.Id = "002";

                GetService(servicename).Widgets.Add(showip);
            }
            else if (servicename == "Github" && widgetname == "Notifications")
            {
                Widget pr = new Widget();
                pr.Size = 2;
                pr.Body = "Widgets/Github/_Pr";
                pr.Name = "Notifications";
                pr.Description = "Show your latest Github notifications";
                pr.Id = "003";

                GetService(servicename).Widgets.Add(pr);
            }
            else if (widgetname == "Cat")
            {
                Widget pr = new Widget();
                pr.Size = 1;
                pr.Body = "Widgets/CatAPI/_Cat";
                pr.Name = "Cat";
                pr.Description = "Show random cat images";
                pr.Id = "004";

                GetService(servicename).Widgets.Add(pr);
            }
        }

        public static Service GetService(string servicename)
        {
            foreach (var service in Services)
            {
                if (service.Name == servicename)
                {
                    return service;
                }
            }
            return null;
        }

        public static Widget GetWidget(string widgetname)
        {
            foreach (var service in Services)
            {
                foreach (var widget in service.Widgets)
                {
                    if (widget.Name == widgetname)
                    {
                        return widget;
                    }
                }
            }
            return null;
        }

        public static bool ContainsService(string servicename)
        {
            foreach (var service in Services)
            {
                if (service.Name == servicename)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool UserContainsService(string servicename, string services)
        {
            var service = GetService(servicename);

            if (service == null)
            {
                return false;
            }
            if (services.Contains(service.Name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool UserContainsWidget(Widget widget, string widgets)
        {
            if (widgets.Contains(widget.Id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetCount(Service service, string subscribedwidgets)
        {
            int counter = 0;

            var subscribedwidgetlist = subscribedwidgets.Split(':');

            foreach (var widget in service.Widgets)
            {
                foreach (var subscribedwidget in subscribedwidgetlist)
                {
                    if (widget.Id == subscribedwidget)
                    {
                        counter++;
                    }
                }
            }

            return service.Widgets.Count - counter;
        }

        public static string[] GetWidgetPos(string wigdets, string widgetid)
        {
            List<string> Items = wigdets.Split(':').Select(i => i.Trim()).Where(i => i != string.Empty).ToList();

            foreach (var item in Items)
            {
                if (item.StartsWith(widgetid))
                {
                    var values = item.Split('=')[1].Split(',');
                    values[0] += "px";
                    values[1] += "px";
                    return values;
                }
            }
            return null;
        }
    }
}
