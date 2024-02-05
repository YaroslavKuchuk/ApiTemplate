using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ActivationCodeService : IActivationCodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ActivationCode> _codeRepository;
        private static readonly Random Random = new Random();

        public ActivationCodeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _codeRepository = _unitOfWork.Repository<ActivationCode>();
        }


        public async Task<string> ReRenderCode(string entity)
        {
            var codes = _codeRepository.GetFilteredQueryable(c => c.EntityActivation == entity).ToList();
            if (codes != null && codes.Count() > 0)
            {
                _codeRepository.DeleteEntities(codes);
            }
            var code = GenerateUniqueCode(4);
            var item = new ActivationCode()
            {
                Code = code,
                IsActivated = false,
                CreateDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.AddDays(30),
                EntityActivation = entity
            };
            await _codeRepository.Insert(item);
            await _unitOfWork.SaveChangesAsync();
            return code;
        }

        public async Task<bool> IsValidateCodeAsync(string code, string entity, DateTime expDate)
        {
            var codes = await _codeRepository.FindByAsync(e => e.Code == code && !e.IsActivated 
                                                            && e.EntityActivation == entity 
                                                            && (!e.ExpireDate.HasValue || e.ExpireDate.Value >= expDate));
            if (codes.Any())
            {
                foreach(var codeItem in codes)
                {
                    codeItem.IsActivated = true;
                    _codeRepository.Update(codeItem);
                }
                await _unitOfWork.SaveChangesAsync();
            }
            return codes.Any();
        }


        public static string GenerateUniqueCode(int size)
        {
            const string chars = "0123456789";
            var result = new string(
                Enumerable.Repeat(chars, size)
                    .Select(s => s[Random.Next(s.Length)])
                    .ToArray());

            return result;
        }

        public async Task<bool> IsActiveCode(string entity)
        {
            var code = await _codeRepository.FindByAsync(c => c.EntityActivation == entity);
            return code != null && code.Any() && code.FirstOrDefault().IsActivated;
        }
    }
}
