namespace CVAggregator.Services
{
    public interface ICurriculumVitaeService
    {
        Page<global::AggregatorService.Domain.CurriculumVitae> Load(QueryCriteria criteria = null);
    }
}
