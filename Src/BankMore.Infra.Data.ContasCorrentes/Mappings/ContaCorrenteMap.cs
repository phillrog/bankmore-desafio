using BankMore.Domain.ContasCorrentes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Infra.Data.ContasCorrentes.Mappings;

public class ContaCorrenteMap : IEntityTypeConfiguration<ContaCorrente>
{
    public void Configure(EntityTypeBuilder<ContaCorrente> builder)
    {
        builder.Property(c => c.Id)
            .HasColumnName("Id");

        builder.Property(c => c.Numero)
            .HasColumnType("integer")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Ativo)
           .HasColumnType("BIT")
           .HasDefaultValue(1);

        builder.Property(c => c.Nome)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Senha)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Salt)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(c => c.Cpf)
            .HasColumnType("varchar(20)")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
