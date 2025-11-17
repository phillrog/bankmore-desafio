using BankMore.Domain.ContasCorrentes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Infra.Data.ContasCorrentes.Mappings
{
    public class IdempotenciaMap : IEntityTypeConfiguration<Idempotencia>
    {
        public void Configure(EntityTypeBuilder<Idempotencia> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.IdContaCorrente)
                   .IsRequired();

            builder.Property(i => i.Requisicao)
                   .HasColumnType("varchar(max)")
                   .IsRequired();

            builder.Property(i => i.Resultado)
                   .HasColumnType("varchar(max)")
                   .IsRequired();

            builder.HasOne(i => i.ContaCorrente)
                   .WithMany()
                   .HasForeignKey(i => i.IdContaCorrente)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("idempotencia");
        }
    }
}