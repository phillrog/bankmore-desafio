using BankMore.Domain.ContasCorrentes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMore.Infra.Data.ContasCorrentes.Mappings
{
    public class MovimentoConfiguration : IEntityTypeConfiguration<Movimento>
    {
        public void Configure(EntityTypeBuilder<Movimento> builder)
        {
            builder.ToTable("movimento");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.IdContaCorrente)
                   .HasColumnName("idcontacorrente")
                   .IsRequired();

            builder.Property(m => m.DataMovimento)
                   .HasColumnName("datamovimento")
                   .HasColumnType("DATETIME2(0)")
                   .IsRequired();

            builder.Property(m => m.TipoMovimento)
                   .HasColumnName("tipomovimento")
                   .HasColumnType("CHAR(1)")
                   .IsRequired();

            builder.Property(m => m.Valor)
                   .HasColumnType("DECIMAL(18, 2)")
                   .IsRequired();

            builder.Property(m => m.Descricao)
                   .HasColumnType("VARCHAR(200)");

            builder.Property(i => i.IdTransferencia)
                   .IsRequired(false);

            builder.HasOne(m => m.ContaCorrente)
                   .WithMany(d => d.Movimentacoes)
                   .HasForeignKey(m => m.IdContaCorrente)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
