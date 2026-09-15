// Kaio Fernandes Branco
using System.ComponentModel.DataAnnotations;
namespace AcademiaDoZe.Application.Enums;
public enum AppColaboradorTipo { [Display(Name = "Administrador")] Administrador, [Display(Name = "Atendente")] Atendente, [Display(Name = "Instrutor")] Instrutor }
public enum AppColaboradorVinculo { [Display(Name = "CLT")] CLT, [Display(Name = "Estagiário")] Estagio }
public enum AppMatriculaPlano { [Display(Name = "Mensal")] Mensal, [Display(Name = "Trimestral")] Trimestral, [Display(Name = "Semestral")] Semestral, [Display(Name = "Anual")] Anual }
[Flags] public enum AppMatriculaRestricoes { [Display(Name = "Nenhuma Restrição")] None, [Display(Name = "Diabetes")] Diabetes, [Display(Name = "Pressão Alta")] PressaoAlta = 2, [Display(Name = "Labirintite")] Labirintite = 4, [Display(Name = "Alergias")] Alergias = 8, [Display(Name = "Problemas Respiratórios")] ProblemasRespiratorios = 16, [Display(Name = "Remédio Contínuo")] RemedioContinuo = 32 }
