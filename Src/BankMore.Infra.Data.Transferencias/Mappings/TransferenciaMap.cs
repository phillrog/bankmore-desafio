using BankMore.Domain.Transferencias.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Infra.Data.Transferencias.Mappings;

public class TransferenciaMap : IEntityTypeConfiguration<Transferencia>
{
    public void Configure(EntityTypeBuilder<Transferencia> builder)
    {
        builder.ToTable("transferencia");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("Id").ValueGeneratedNever();

        builder.Property(c => c.DataMovimento)
            .HasColumnType("DATETIME2(0)")
            .IsRequired();

        builder.Property(c => c.Status)
           .HasColumnType("integer")
           .HasConversion<int>()
           .IsRequired();

        builder.Property(c => c.IdContaCorrenteOrigem)
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(c => c.IdContaCorrenteDestino)
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(m => m.Valor)
                   .HasColumnType("DECIMAL(10, 2)")
                   .IsRequired();

        builder.Property(c => c.DataUltimaAlteracao)
            .HasColumnType("DATETIME2(0)")
            .IsRequired(false);

        builder.Property(c => c.Erro)
            .HasColumnType("varchar(max)")
            .IsRequired(false);

        builder.Ignore(c => c.StatusDescricao);
    }
}
