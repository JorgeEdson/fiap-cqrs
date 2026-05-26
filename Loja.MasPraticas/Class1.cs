namespace Loja.MasPraticas
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Net.Http.Json;
    using System.Text.Json.Serialization;

    namespace Sistema.Legado.Dominio.Entidades;

    /// <summary>
    /// EXEMPLO DIDÁTICO.
    /// ESTA CLASSE FOI FEITA INTENCIONALMENTE CHEIA DE MÁS PRÁTICAS.
    /// NÃO USE ISSO EM PRODUÇÃO.
    /// </summary>
    public class Pedido
    {
        // =========================================================
        // DADOS DE BANCO
        // =========================================================

        [Key]
        public long Id { get; set; }

        public Guid CodigoExterno { get; set; }

        [Required]
        [MaxLength(255)]
        public string NumeroPedido { get; set; }

        [Required]
        [MaxLength(500)]
        public string NomeCliente { get; set; }

        [MaxLength(14)]
        public string CpfCliente { get; set; }

        [MaxLength(20)]
        public string TelefoneCliente { get; set; }

        [MaxLength(255)]
        public string EmailCliente { get; set; }

        [MaxLength(255)]
        public string Endereco { get; set; }

        [MaxLength(255)]
        public string Bairro { get; set; }

        [MaxLength(255)]
        public string Cidade { get; set; }

        [MaxLength(2)]
        public string Estado { get; set; }

        [MaxLength(8)]
        public string Cep { get; set; }

        public decimal ValorProduto { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorImposto { get; set; }

        public decimal ValorDesconto { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal PesoTotal { get; set; }

        public decimal AlturaPacote { get; set; }

        public decimal LarguraPacote { get; set; }

        public decimal ComprimentoPacote { get; set; }

        public int QuantidadeItens { get; set; }

        public bool Pago { get; set; }

        public bool Cancelado { get; set; }

        public bool Enviado { get; set; }

        public bool Entregue { get; set; }

        public bool ClienteVip { get; set; }

        public bool PossuiFraude { get; set; }

        public bool UsuarioAlterouManualmenteFrete { get; set; }

        public bool PedidoImportadoMarketplace { get; set; }

        public bool IntegradoSap { get; set; }

        public bool IntegradoErp { get; set; }

        public bool IntegradoCorreios { get; set; }

        public bool IntegradoGatewayPagamento { get; set; }

        public bool PrecisaReprocessar { get; set; }

        public bool Reprocessado { get; set; }

        public bool ExibirBannerPromocional { get; set; }

        public bool ExibirAlertaFinanceiro { get; set; }

        public bool PermitirEdicaoTela { get; set; }

        public bool UsuarioPodeCancelar { get; set; }

        public bool UsuarioPodeEditarEndereco { get; set; }

        public bool UsuarioPodeAlterarValor { get; set; }

        public bool UsuarioPodeVerCustosInternos { get; set; }

        public bool GerarLogCompleto { get; set; }

        public bool EnviarEmailCliente { get; set; }

        public bool EnviarSmsCliente { get; set; }

        public bool EnviarWebhook { get; set; }

        public bool GerarPdfDanfe { get; set; }

        public bool GerarEtiqueta { get; set; }

        public bool GerarXmlNfe { get; set; }

        public bool AtualizarEstoque { get; set; }

        public bool ProdutoImportado { get; set; }

        public bool ProdutoControlado { get; set; }

        public bool PedidoUrgente { get; set; }

        public bool PedidoInternacional { get; set; }

        public bool PedidoSuspeito { get; set; }

        public bool PrecisaValidacaoManual { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataPagamento { get; set; }

        public DateTime? DataEnvio { get; set; }

        public DateTime? DataEntrega { get; set; }

        public DateTime? DataCancelamento { get; set; }

        public DateTime? DataIntegracaoSap { get; set; }

        public DateTime? DataIntegracaoErp { get; set; }

        public DateTime? DataUltimaAlteracao { get; set; }

        public DateTime? DataUltimoErroIntegracao { get; set; }

        // =========================================================
        // CONFIGURAÇÕES DE SERIALIZAÇÃO
        // =========================================================

        [JsonIgnore]
        public string TokenInternoGatewayPagamento { get; set; }

        [JsonIgnore]
        public string SenhaApiCorreios { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string ConnectionStringTemporaria { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatusPedido StatusPedido { get; set; }

        // =========================================================
        // PROPRIEDADES PARA TELA
        // =========================================================

        [NotMapped]
        public string NomeClienteFormatadoTela
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeCliente))
                    return "CLIENTE NÃO INFORMADO";

                return NomeCliente.Trim().ToUpper();
            }
        }

        [NotMapped]
        public string CpfFormatadoTela
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CpfCliente))
                    return "";

                var cpf = CpfCliente.Replace(".", "").Replace("-", "");

                if (cpf.Length != 11)
                    return cpf;

                return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
            }
        }

        [NotMapped]
        public string TelefoneFormatadoTela
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TelefoneCliente))
                    return "";

                var telefone = TelefoneCliente
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("-", "")
                    .Replace(" ", "");

                if (telefone.Length == 11)
                {
                    return Convert.ToUInt64(telefone)
                        .ToString(@"\(00\) 00000\-0000");
                }

                return telefone;
            }
        }

        [NotMapped]
        public string ValorTotalFormatado
        {
            get
            {
                return $"R$ {ValorTotal:N2}";
            }
        }

        [NotMapped]
        public string BadgeStatusTela
        {
            get
            {
                if (Cancelado)
                    return "badge-danger";

                if (Entregue)
                    return "badge-success";

                if (Enviado)
                    return "badge-info";

                return "badge-warning";
            }
        }

        [NotMapped]
        public string CorStatusTela
        {
            get
            {
                if (Cancelado)
                    return "#ff0000";

                if (Entregue)
                    return "#00ff00";

                return "#cccccc";
            }
        }

        // =========================================================
        // REGRAS DE NEGÓCIO MISTURADAS
        // =========================================================

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(NomeCliente))
                throw new Exception("Nome do cliente obrigatório.");

            if (string.IsNullOrWhiteSpace(CpfCliente))
                throw new Exception("CPF obrigatório.");

            if (ValorProduto <= 0)
                throw new Exception("Valor inválido.");

            if (Cancelado && Pago)
                throw new Exception("Pedido cancelado não pode estar pago.");

            if (Entregue && !Enviado)
                throw new Exception("Pedido entregue precisa estar enviado.");

            if (PedidoInternacional && string.IsNullOrWhiteSpace(PassaporteCliente()))
                throw new Exception("Pedido internacional exige passaporte.");

            if (PedidoUrgente && ValorFrete <= 0)
                throw new Exception("Pedido urgente exige frete.");

            if (ClienteVip && ValorProduto < 100)
                throw new Exception("Cliente VIP precisa ter pedido mínimo.");

            if (CpfCliente.Length < 11)
                throw new Exception("CPF inválido.");

            if (CpfCliente == "00000000000")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "11111111111")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "22222222222")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "33333333333")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "44444444444")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "55555555555")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "66666666666")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "77777777777")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "88888888888")
                throw new Exception("CPF inválido.");

            if (CpfCliente == "99999999999")
                throw new Exception("CPF inválido.");

            if (string.IsNullOrWhiteSpace(Endereco))
                throw new Exception("Endereço obrigatório.");

            if (string.IsNullOrWhiteSpace(Cidade))
                throw new Exception("Cidade obrigatória.");

            if (string.IsNullOrWhiteSpace(Estado))
                throw new Exception("Estado obrigatório.");

            if (ValorTotal > 10000 && !ClienteVip)
                throw new Exception("Pedido acima do permitido.");

            if (PossuiFraude)
                throw new Exception("Pedido bloqueado por fraude.");

            if (PedidoSuspeito)
                throw new Exception("Pedido suspeito.");

            if (PesoTotal > 100)
                throw new Exception("Peso inválido.");

            if (AlturaPacote > 200)
                throw new Exception("Altura inválida.");

            if (LarguraPacote > 200)
                throw new Exception("Largura inválida.");

            if (ComprimentoPacote > 200)
                throw new Exception("Comprimento inválido.");

            if (QuantidadeItens <= 0)
                throw new Exception("Quantidade inválida.");

            if (ValorDesconto > ValorProduto)
                throw new Exception("Desconto inválido.");
        }

        // =========================================================
        // LÓGICA DE IMPOSTO
        // =========================================================

        public void CalcularImposto()
        {
            if (Estado == "CE")
            {
                ValorImposto = ValorProduto * 0.18m;
            }
            else if (Estado == "SP")
            {
                ValorImposto = ValorProduto * 0.12m;
            }
            else if (Estado == "RJ")
            {
                ValorImposto = ValorProduto * 0.15m;
            }
            else
            {
                ValorImposto = ValorProduto * 0.10m;
            }

            if (ClienteVip)
            {
                ValorImposto -= 5;
            }

            if (PedidoInternacional)
            {
                ValorImposto += 100;
            }

            if (ProdutoControlado)
            {
                ValorImposto += 50;
            }

            if (ValorImposto < 0)
            {
                ValorImposto = 0;
            }
        }

        // =========================================================
        // CÁLCULO DE FRETE
        // =========================================================

        public void CalcularFrete()
        {
            if (UsuarioAlterouManualmenteFrete)
                return;

            ValorFrete = 0;

            if (PesoTotal <= 1)
                ValorFrete += 15;

            if (PesoTotal > 1 && PesoTotal <= 5)
                ValorFrete += 30;

            if (PesoTotal > 5)
                ValorFrete += 80;

            if (Estado == "CE")
                ValorFrete += 5;

            if (Estado == "SP")
                ValorFrete += 15;

            if (Estado == "AM")
                ValorFrete += 60;

            if (PedidoUrgente)
                ValorFrete *= 2;

            if (ClienteVip)
                ValorFrete -= 10;

            if (ValorFrete < 0)
                ValorFrete = 0;
        }

        // =========================================================
        // MÉTODOS DE FORMATAÇÃO
        // =========================================================

        public string FormatarCpf()
        {
            if (string.IsNullOrWhiteSpace(CpfCliente))
                return "";

            var cpf = CpfCliente
                .Replace(".", "")
                .Replace("-", "");

            if (cpf.Length != 11)
                return cpf;

            return Convert.ToUInt64(cpf)
                .ToString(@"000\.000\.000\-00");
        }

        public string FormatarCep()
        {
            if (string.IsNullOrWhiteSpace(Cep))
                return "";

            var cep = Cep.Replace("-", "");

            if (cep.Length != 8)
                return cep;

            return Convert.ToUInt64(cep)
                .ToString(@"00000\-000");
        }

        public string FormatarTelefone()
        {
            if (string.IsNullOrWhiteSpace(TelefoneCliente))
                return "";

            var telefone = TelefoneCliente
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "")
                .Replace(" ", "");

            if (telefone.Length == 11)
            {
                return Convert.ToUInt64(telefone)
                    .ToString(@"\(00\) 00000\-0000");
            }

            return telefone;
        }

        // =========================================================
        // SERIALIZAÇÃO MANUAL
        // =========================================================

        public string SerializarParaTela()
        {
            return JsonConvert.SerializeObject(new
            {
                id = Id,
                numero = NumeroPedido,
                cliente = NomeClienteFormatadoTela,
                cpf = CpfFormatadoTela,
                telefone = TelefoneFormatadoTela,
                valor = ValorTotalFormatado,
                status = StatusPedido.ToString(),
                badge = BadgeStatusTela,
                cor = CorStatusTela,
                podeEditar = PermitirEdicaoTela,
                podeCancelar = UsuarioPodeCancelar,
                dataCriacao = DataCriacao.ToString("dd/MM/yyyy HH:mm:ss")
            });
        }

        public string SerializarParaIntegracaoSap()
        {
            return JsonConvert.SerializeObject(new
            {
                pedido = NumeroPedido,
                cliente = NomeCliente,
                cpf = CpfCliente,
                valor = ValorTotal,
                imposto = ValorImposto,
                frete = ValorFrete,
                cep = Cep,
                endereco = Endereco,
                cidade = Cidade,
                estado = Estado
            });
        }

        public string SerializarParaWebhook()
        {
            return JsonConvert.SerializeObject(this);
        }

        // =========================================================
        // LÓGICA DE STATUS
        // =========================================================

        public void AtualizarStatus()
        {
            if (Cancelado)
            {
                StatusPedido = StatusPedido.Cancelado;
                return;
            }

            if (Entregue)
            {
                StatusPedido = StatusPedido.Entregue;
                return;
            }

            if (Enviado)
            {
                StatusPedido = StatusPedido.Enviado;
                return;
            }

            if (Pago)
            {
                StatusPedido = StatusPedido.Pago;
                return;
            }

            StatusPedido = StatusPedido.AguardandoPagamento;
        }

        // =========================================================
        // REGRAS DE TELA
        // =========================================================

        public bool DeveExibirBannerBlackFriday()
        {
            if (DataCriacao.Month == 11 &&
                DataCriacao.Day >= 20 &&
                DataCriacao.Day <= 30)
            {
                return true;
            }

            return false;
        }

        public bool DeveExibirBotaoCancelar()
        {
            if (Cancelado)
                return false;

            if (Entregue)
                return false;

            if (Enviado)
                return false;

            return true;
        }

        public bool DeveExibirMensagemFraude()
        {
            return PossuiFraude || PedidoSuspeito;
        }

        // =========================================================
        // MÉTODOS AUXILIARES BIZARROS
        // =========================================================

        private string PassaporteCliente()
        {
            return "";
        }

        public void ProcessarPedido()
        {
            Validar();

            CalcularImposto();

            CalcularFrete();

            ValorTotal =
                ValorProduto +
                ValorFrete +
                ValorImposto -
                ValorDesconto;

            AtualizarStatus();

            GerarLogs();

            if (EnviarEmailCliente)
            {
                EnviarEmail();
            }

            if (EnviarSmsCliente)
            {
                EnviarSms();
            }

            if (EnviarWebhook)
            {
                DispararWebhook();
            }

            if (AtualizarEstoque)
            {
                AtualizarEstoqueSistema();
            }

            if (IntegradoSap)
            {
                IntegrarSap();
            }

            if (IntegradoErp)
            {
                IntegrarErp();
            }
        }

        // =========================================================
        // ACOPLAMENTO EXTERNO
        // =========================================================

        public void EnviarEmail()
        {
            Console.WriteLine("Enviando email...");
        }

        public void EnviarSms()
        {
            Console.WriteLine("Enviando SMS...");
        }

        public void DispararWebhook()
        {
            Console.WriteLine("Disparando webhook...");
        }

        public void AtualizarEstoqueSistema()
        {
            Console.WriteLine("Atualizando estoque...");
        }

        public void IntegrarSap()
        {
            Console.WriteLine("Integrando SAP...");
        }

        public void IntegrarErp()
        {
            Console.WriteLine("Integrando ERP...");
        }

        public void GerarLogs()
        {
            if (!GerarLogCompleto)
                return;

            Console.WriteLine("=================================================");
            Console.WriteLine($"Pedido: {NumeroPedido}");
            Console.WriteLine($"Cliente: {NomeCliente}");
            Console.WriteLine($"CPF: {CpfCliente}");
            Console.WriteLine($"Valor Produto: {ValorProduto}");
            Console.WriteLine($"Valor Frete: {ValorFrete}");
            Console.WriteLine($"Valor Imposto: {ValorImposto}");
            Console.WriteLine($"Valor Total: {ValorTotal}");
            Console.WriteLine($"Data: {DateTime.Now}");
            Console.WriteLine("=================================================");
        }

        // =========================================================
        // MÉTODOS DUPLICADOS E DESNECESSÁRIOS
        // =========================================================

        public bool ClienteEhVip()
        {
            return ClienteVip;
        }

        public bool PedidoEstaPago()
        {
            return Pago;
        }

        public bool PedidoFoiEnviado()
        {
            return Enviado;
        }

        public bool PedidoFoiCancelado()
        {
            return Cancelado;
        }

        public bool PedidoFoiEntregue()
        {
            return Entregue;
        }

        public string ObterNomeClienteMaiusculo()
        {
            if (string.IsNullOrWhiteSpace(NomeCliente))
                return "";

            return NomeCliente.ToUpper();
        }

        public string ObterEnderecoCompleto()
        {
            return $"{Endereco} - {Bairro} - {Cidade}/{Estado}";
        }

        public decimal ObterValorFinal()
        {
            return ValorProduto + ValorFrete + ValorImposto - ValorDesconto;
        }

        public bool PedidoPodeSerEditado()
        {
            if (Cancelado)
                return false;

            if (Entregue)
                return false;

            return true;
        }

        public bool PedidoPodeSerExcluido()
        {
            if (Pago)
                return false;

            if (Enviado)
                return false;

            return true;
        }

        // =========================================================
        // GAMBIARRAS LEGADAS
        // =========================================================

        [Obsolete("Não remover porque o relatório antigo usa isso.")]
        public string NomeClienteRelatorioLegado
        {
            get
            {
                return NomeCliente;
            }
        }

        [Obsolete("Sistema legado da transportadora.")]
        public string CepSemMascara
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Cep))
                    return "";

                return Cep.Replace("-", "");
            }
        }

        [Obsolete("Tela ASPX antiga.")]
        public string StatusTextoAntigo
        {
            get
            {
                return StatusPedido.ToString().ToUpper();
            }
        }

        // =========================================================
        // MAIS COISAS SEM SENTIDO
        // =========================================================

        public Dictionary<string, object> CacheTemporario { get; set; }
            = new();

        public List<string> LogsInternos { get; set; }
            = new();

        public string HtmlRenderizadoTela { get; set; }

        public string CssTela { get; set; }

        public string JavascriptTela { get; set; }

        public string XmlNfeGerado { get; set; }

        public string JsonIntegracaoTemporario { get; set; }

        public byte[] PdfDanfeBytes { get; set; }

        public string StackTraceUltimoErro { get; set; }

        public string MensagemUltimoErro { get; set; }

        public int TentativasReprocessamento { get; set; }

        public int TimeoutIntegracao { get; set; }

        public int QuantidadeErros { get; set; }

        public string UsuarioUltimaAlteracao { get; set; }

        public string UsuarioCriacao { get; set; }

        public string IpUltimaAlteracao { get; set; }

        public string NavegadorUltimaAlteracao { get; set; }

        public string SistemaOperacionalUltimaAlteracao { get; set; }

        public string ObservacoesInternas { get; set; }

        public string ObservacoesFinanceiro { get; set; }

        public string ObservacoesExpedicao { get; set; }

        public string ObservacoesCliente { get; set; }

        public string ComentarioMarketplace { get; set; }

        public string CodigoMarketplace { get; set; }

        public string CodigoSap { get; set; }

        public string CodigoErp { get; set; }

        public string CodigoTransportadora { get; set; }

        public string CodigoRastreamento { get; set; }

        public string UrlEtiqueta { get; set; }

        public string UrlNotaFiscal { get; set; }

        public string UrlBoleto { get; set; }

        public string UrlPix { get; set; }

        public string UrlComprovantePagamento { get; set; }
    }

    public enum StatusPedido
    {
        AguardandoPagamento = 1,
        Pago = 2,
        Enviado = 3,
        Entregue = 4,
        Cancelado = 5
    }
}
