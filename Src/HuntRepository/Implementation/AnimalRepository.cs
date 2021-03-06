using System;
using System.Collections.Generic;
using System.Linq;
using GravityZero.HuntingSupport.Repository.Configuration;
using GravityZero.HuntingSupport.Repository.Infrastructure;
using GravityZero.HuntingSupport.Repository.Model;
using log4net;
using Microsoft.EntityFrameworkCore.Storage;

namespace GravityZero.HuntingSupport.Repository
{
    public class AnimalRepository : IAnimalRepository
    {

        private readonly HuntContext context;
        private readonly ILog log = LogManager.GetLogger(typeof(AnimalRepository));
        private string TAG = "RA";

        public AnimalRepository(HuntContext context)
        {
            this.context = context;
            LoggerConfig.ReadConfiguration();
        }

        public RepositoryResult<Animal> Add(Animal animal)
        {
            //var result = new RepositoryResult<Animal>(false, new Animal());
            IDbContextTransaction tx = null;
            try{
                tx = context.Database.BeginTransaction();
                animal.Identifier = Guid.NewGuid();
                context.Animals.Add(animal);
                context.SaveChanges();
                tx.Commit();
                log.Info($"Dodano zwierzyne {animal}");
                return new RepositoryResult<Animal>(true, animal, TAG+"01");
                
            }
            catch(Exception ex){
                log.Error($"Nie udało dodać sie zwierzyny {animal}, {ex}");
                return new RepositoryResult<Animal>(true, null, TAG+"02");;
            }
            finally{
                tx?.Dispose();
            }           
        }

        public RepositoryResult<string> Delete(Guid identifier)
        {
            //var result = new RepositoryResult<string>(false, "",TAG);
            var tmpAnimal = context.Animals.Find(identifier);
            if(tmpAnimal!=null){
                IDbContextTransaction tx = null;
                try{
                    tx = context.Database.BeginTransaction();
                    context.Animals.Remove(tmpAnimal);
                    context.SaveChanges();
                    tx.Commit();
                    log.Info($"Usunięto zwierzyne: {identifier}");
                    return new RepositoryResult<string>(true,"",TAG+"05");
                }
                catch(Exception ex){
                    log.Error($"Nie udało usunac się zwierzyny {identifier}, {ex}");
                    return new RepositoryResult<string>(false, ex.Message.ToString(), TAG+"06");
                }
                finally{
                    tx?.Dispose();
                }    
            }
            else{
                return new RepositoryResult<string>(false, "", TAG+"11");
            }
        }

        public RepositoryResult<Animal> Find(Guid identifier)
        {
            try{
                var found = context.Animals.Find(identifier);
                return found != null ?
                                new RepositoryResult<Animal>(true, found, TAG+"07"):
                                new RepositoryResult<Animal>(false, null, TAG+"08");
                            
            }
            catch(Exception ex){
                log.Error($"{ex}");
                return new RepositoryResult<Animal>(false, null);
            }
            finally{}
        }

        public RepositoryResult<IEnumerable<Animal>> Query(Func<Animal, bool> query)
        {
            RepositoryResult<IEnumerable<Animal>> result = new RepositoryResult<IEnumerable<Animal>>(false, new List<Animal>());
            IDbContextTransaction tx = null;
            try{
                var resultQuery = context.Animals.Where(ux=>query.Invoke(ux));
                return new RepositoryResult<IEnumerable<Animal>>(true, resultQuery.AsEnumerable(), TAG+"09");
            }
            catch(Exception ex){
                log.Error($"Zapytanie nie powiodło się {query}, {ex}");
                return new RepositoryResult<IEnumerable<Animal>>(true, null, TAG+"10");;
            }
            finally{              
                tx?.Dispose();
            }
        }

        public RepositoryResult<Animal> Update(Animal animal)
        {
            //var result = new RepositoryResult<Animal>(false, null,TAG);
            var tmpAnimal = context.Animals.Find(animal.Identifier);
            if(tmpAnimal!=null){
                IDbContextTransaction tx = null;
                try{
                    tx = context.Database.BeginTransaction();
                    tmpAnimal.Name = animal.Name;
                    context.SaveChanges();
                    tx.Commit();
                    log.Info($"Zaktualizowano zwierzyne {animal}");
                    return new RepositoryResult<Animal>(true,tmpAnimal,TAG+"03");
                }
                catch(Exception ex){
                    log.Error($"NIe powiodła się aktualizacja {animal}, {ex}");
                    return new RepositoryResult<Animal>(false, null, TAG+"04");
                }
                finally{
                    tx?.Dispose();
                }
            }
            else{
                return new RepositoryResult<Animal>(false,null, TAG+"12");
            }
        }
    }
}