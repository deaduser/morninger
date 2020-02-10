﻿namespace Morninger
{
    using System.Net;
    using System;
    using Telegram.Bot.Args;
    using Telegram.Bot;

    public class Program
    {
        private static ITelegramBotClient botClient;
        private static SpeakerService speaker;
        private static SQLiteProvider db;

        static void Main(string[] args)
        {
            db = new SQLiteProvider(args[0]);
            speaker = new SpeakerService();

            var httpProxy = args[1] != string.Empty
                ? new WebProxy(args[1])
                : null;

            botClient = httpProxy == null
                ? new TelegramBotClient(args[2])
                : new TelegramBotClient(args[2], httpProxy);

            botClient.SetWebhookAsync("");
            botClient.OnMessage += onMessage;
            botClient.StartReceiving();
            Console.WriteLine("App started...");

            while (Console.ReadLine() != "Exit") ;
        }

        static void onMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null && e.Message.Text != string.Empty)
            {
                string answer = string.Empty;

                try
                {
                    answer = speaker.ProcessMessage(db, e.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    botClient.SendTextMessageAsync(e.Message.Chat.Id, ex.Message);
                }

                if (answer != string.Empty)
                {
                    botClient.SendTextMessageAsync(e.Message.Chat.Id, answer);
                }
            }
        }
    }
}