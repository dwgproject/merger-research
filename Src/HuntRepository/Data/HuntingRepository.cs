using System;
using System.Collections.Generic;
using System.Linq;
using Hunt.Configuration;
using Hunt.Data;
using Hunt.Model;
using HuntRepository.Infrastructure;
using log4net;
using Microsoft.EntityFrameworkCore.Storage;

namespace HuntRepository.Data
{
    public class HuntingRepository : IHuntingRepository
    {
        private readonly HuntContext context;
        private readonly ILog log = LogManager.GetLogger(typeof(HuntingRepository));

        public HuntingRepository(HuntContext context)
        {
            this.context = context;
            LoggerConfig.ReadConfiguration();
        }

        public Result<Hunting> Add(Hunting hunting)
        {

            var result = new Result<Hunting>(false, new Hunting());
            IDbContextTransaction tx = null;   
            try{
                tx = context.Database.BeginTransaction();
                hunting.Identifier = Guid.NewGuid();
                hunting.Issued = DateTime.Now;
                hunting.Leader = context.Users.FirstOrDefault(x=>x.Identifier == hunting.Leader.Identifier);
                hunting.Status = true;
                context.Huntings.Add(hunting);
                context.SaveChanges();
                tx.Commit();
                result = new Result<Hunting>(true, hunting);
                log.Info($"Dodano nowe polowanie {hunting}");
                return result;
            }
            catch(Exception ex){
                log.Error($"Nie udało sie dodać nowego polowania {hunting}, {ex}");
                return result;
            }
            finally{
                tx?.Dispose();
            }
            
        }

        public void Delete(Guid identifier)
        {
            var tmpHunting = context.Huntings.Find(identifier);
            if(tmpHunting!=null){
                IDbContextTransaction tx=null;
                try{
                    tx = context.Database.BeginTransaction();
                    context.Remove(tmpHunting);
                    context.SaveChanges();
                    tx.Commit();
                    log.Info($"Usunięto polowanie {identifier}");
                }
                catch(Exception ex){
                    log.Error($"Nie udało się usunąc polowania {identifier}, {ex}");
                }
                finally{
                    tx?.Dispose();
                }
            }
        }

        public Result<Hunting> Find(Guid identifier)
        {
            throw new NotImplementedException();
        }

        public Result<Hunting> Finish(Hunting hunting)
        {
            var result = new Result<Hunting>(false, new Hunting());
            var tmpHunting = context.Huntings.Find(hunting.Identifier);
            if(tmpHunting!=null){
                IDbContextTransaction tx = null;
                try{
                    tx = context.Database.BeginTransaction();
                    tmpHunting.Status=false;
                    context.SaveChanges();
                    tx.Commit();
                    result = new Result<Hunting>(true, tmpHunting);
                    return result;
                }
                catch(Exception ex){
                    log.Error($"Zakoczenie polowania nie powiodło się {hunting}, {ex}");
                    return result;
                }
                finally{
                    tx?.Dispose();
                }
            }
            else{
                return result;

            }
        }

        public Result<IEnumerable<Hunting>> Query(Func<Hunting, bool> query)
        {
            var result = new Result<IEnumerable<Hunting>>(false, new List<Hunting>());
            IDbContextTransaction tx = null;
            try{
                var resultQuery = context.Huntings.Where(ux=>query.Invoke(ux));
                return new Result<IEnumerable<Hunting>>(true, resultQuery.AsEnumerable());
            }
            catch(Exception ex){
                log.Error($"Zapytanie nie powiodło się {query}, {ex}");
                return result;
            }
            finally{
                tx?.Dispose();
            }

        }

        public void Update(Hunting hunting)
        {
            var tmpHunting = context.Huntings.Find(hunting.Identifier);
            if(tmpHunting!=null && tmpHunting.Status!=false)
            {
                IDbContextTransaction tx = null;
                try{
                    tx = context.Database.BeginTransaction();
                    tmpHunting.Leader = hunting.Leader;
                    context.SaveChanges();
                    tx.Commit();
                    log.Info($"Zaktualizowano polowanie {hunting}");
                }
                catch(Exception ex){
                    log.Error($"Nie udało się zaktuaizować polowania {hunting}, {ex}");
                }
                finally{
                    tx?.Dispose();
                }
            }
        }
    }
}