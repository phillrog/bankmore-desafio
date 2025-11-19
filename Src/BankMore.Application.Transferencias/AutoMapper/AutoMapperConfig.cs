namespace BankMore.Application.Transferencias.AutoMapper;

public class AutoMapperConfig
{
    public static Type[] RegisterMappings()
    {
        return new Type[]
        {
            typeof(DomainToViewModelMappingProfile),
            typeof(ViewModelToDomainMappingProfile),
        };
    }
}
