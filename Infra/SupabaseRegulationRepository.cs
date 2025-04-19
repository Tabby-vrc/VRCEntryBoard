using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

using VRCEntryBoard.Infra.Logger;
using VRCEntryBoard.HMI.Exception;
using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.Infra
{
    internal sealed class SupabaseRegulationRepository : IRegulationRepository
    {
        private readonly ILogger<SupabaseRegulationRepository> _logger;
        private readonly IExceptionNotifier _exceptionNotifier;
        private readonly Supabase.Client _supabaseClient;
        private List<Regulation> _regulations;

        public SupabaseRegulationRepository(SupabaseClient supabaseClient, IExceptionNotifier exceptionNotifier)
        {
            _exceptionNotifier = exceptionNotifier ?? 
                throw new ArgumentNullException(nameof(exceptionNotifier));
            _logger = LogManager.GetLogger<SupabaseRegulationRepository>();
            _regulations = new List<Regulation>();
            _supabaseClient = supabaseClient.GetClient();
        }

        public async Task UpdateRegulations()
        {
            var result = await _supabaseClient.From<RegulationModel>().Get();

            if (result.Models == null || result.Models.Count == 0)
            {
                _logger.LogWarning("取得対象の規定データが見つかりません");
                return;
            }

            result.Models.ForEach(model => 
            {
                _regulations.Add(new Regulation(model.ID, model.RegulationName));
            });
        }

        public List<Regulation> GetRegulations()
        {
            return _regulations;
        }
    }

    [Table("Regulation")]
    internal class RegulationModel : BaseModel
    {
        [PrimaryKey("ID")]
        public int ID { get; set; }
        [Column("RegulationName")]
        public string RegulationName { get; set; }
    }
}
