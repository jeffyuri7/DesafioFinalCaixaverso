using System;
using System.Collections.Generic;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class TelemetriaResumoJson
{
    public IList<TelemetriaServicosJson> Servicos { get; set; } = new List<TelemetriaServicosJson>();
    public TelemetriaPeriodoJson Periodo { get; set; } = new();
}

public class TelemetriaPeriodoJson
{
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
}
