using GravityZero.HuntingSupport.Repository.Model;
using GravityZero.HuntingSupport.Service.Context.Domain;

namespace GravityZero.HuntingSupport.Service.Context.Extensions
{
    public static class UserConverter
    {
        public static User ConverToUserRepository(this FullUser fullUser){
            return new User(){
                Identifier = fullUser.Identifier,
                Name = fullUser.Name,
                Surname = fullUser.Surname,
                Email = fullUser.Email,
                Password = fullUser.Password,
                Login = fullUser.Login,
                Role = fullUser.Role.ConvertToModel()
            };
        }
        
        public static UserServiceModel ConverToUserService(this User model){
            return new UserServiceModel(){
                Name = model.Name,
                Surname = model.Surname,
                Login = model.Login,
                Email = model.Email,
                Role =  new RoleServiceModel().ConvertToServiceRole(model.Role),        
                Identifier = model.Identifier
            };
        }

        public static FullUser ConverToFullUser(this User model){
            return new FullUser(){
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                Identifier = model.Identifier,
                Login = model.Login,
                Password = model.Password,
            };
        }

        public static User ConvertToUserModel(this UserServiceModel model){
            if(model is null)
                return new User();
            return new User(){
                Login = model.Login,
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                Identifier = model.Identifier                
            };           
        }       
    }
}