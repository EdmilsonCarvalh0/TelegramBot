using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Domain;

namespace TelegramBot.UserInterface
{
    public class ResponseContent
    {
        public string Text { get; set; } = string.Empty;
        public InlineKeyboardMarkup? KeyboardMarkup { get; set; }
        public UserState UserState { get; set; }
        public string AdditionalResponseContext { get; set; } = string.Empty;
        private Dictionary<string, string> ResponseMessage { get; set; } = new();
        private Dictionary<string, string> ResponseCallback { get; set; } = new();
        private Dictionary<string, InlineKeyboardMarkup> ResponseInlineKeyboardMarkup { get; set; } = new();

        public ResponseContent()
        {
            LoadResponses();
        }

        public void ResetResponseContent()
        {
            Text = string.Empty;
            KeyboardMarkup = null;
            UserState = UserState.None;
            AdditionalResponseContext = string.Empty;
        }

        private void LoadResponses()
        {
            //TODO: Implement response content in Json file
            //      Implementar conteúdos de resposta em arquivo Json
            ResponseMessage = new Dictionary<string, string>()
            {
                { "Initial Message", "Para ir para o menu inicial digite 'Menu'." },
                { "Menu", $"Olá, bem vindo ao Bot de Compras Mensais!\nEscolha uma opção:" },
                { "Item Added", $"Pronto, já adicionei!" },
                { "Update Item", "O que deseja alterar " },
                { "Non-existent Item", $"Infelizmente não encontrei o item na lista.\n\n" },
                { "Non-existent Item 2", "\n\nVerifique se o nome está correto e informe novamente." },
                { "Update Item OK", "Pronto, alterei pra você." },
                { "Deleted Item OK", "Item removido." },
                { "Created New List", "Nova lista criada." }
            };

            ResponseCallback = new Dictionary<string, string>()
            {
                { "Show List", $"Essa é a sua lista atual:\n\n" },
                { "Atualizar lista",  "Selecione o tipo de atualização que deseja fazer:" },
                { "Adicionar um item",  "Ótimo! Para fazer a inserção você pode enviar um ou mais itens se desejar.\n" + 
                                        "Lembrando que apenas o nome é obrigatório, mas é ideal que você informe a Marca e o Preço também para uma melhor experiência!\n\n" +
                                        "Vamos lá! Para inserir o item, siga a seguinte extrutura:\nProduto - Marca - Preco\nProduto - Marca - Preco\n\nAgora, por favor, informe os itens que deseja adicionar:" },
                { "Alterar um item",  "Por favor, informe agora o nome do item que deseja fazer alteração:" },
                { "Atributte Change",  "Informe " },
                { "Remover um item",  "Por favor, informe agora o nome do item que deseja remover:" },
                { "Criar nova lista",  "Envie agora os itens da nova lista.\nUse o padrão adequado para registro correto dos itens. Nesse caso, separe os itens por vírgula sem espaços.\nEx: Pão,Manteiga,Queijo" }
            };

            ResponseInlineKeyboardMarkup = new Dictionary<string, InlineKeyboardMarkup>()
            {
                { "Menu", new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Ver lista"),
                            InlineKeyboardButton.WithCallbackData("Atualizar lista"),
                            InlineKeyboardButton.WithCallbackData("Criar nova lista")
                        }
                    })
                },
                { "Atualizar lista", new InlineKeyboardMarkup(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Adicionar um item"),
                        },
                        [
                            InlineKeyboardButton.WithCallbackData("Alterar um item")
                        ],
                        [
                            InlineKeyboardButton.WithCallbackData("Remover um item")
                        ]
                    })
                },
                { "Update Item", new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Nome"),
                            InlineKeyboardButton.WithCallbackData("Marca"),
                            InlineKeyboardButton.WithCallbackData("Preço")
                        }
                    })
                
                }
            };

        }

        public ResponseContent GetResponseCallback(string subject)
        {
            return new ResponseContent
            {
                Text = ResponseCallback[subject],
                KeyboardMarkup = ResponseInlineKeyboardMarkup.ContainsKey(subject) ?
                                ResponseInlineKeyboardMarkup[subject] :
                                null
            };
        }

        public ResponseContent GetResponseMessage(string subject)
        {
            return new ResponseContent
            {
                Text = ResponseMessage[subject],
                KeyboardMarkup = ResponseInlineKeyboardMarkup.ContainsKey(subject) ?
                                ResponseInlineKeyboardMarkup[subject] :
                                null
            };
        }
    }
}
