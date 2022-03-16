﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Sample.Elasticsearch.Infrastructure.Indices;

namespace Sample.Elasticsearch.Infrastructure.Abstractions;

public static class NestExtensions
{
    public static QueryContainer BuildMultiMatchQuery<T>(string queryValue) where T : class
    {
        var fields = typeof(T).GetProperties().Select(p => p.Name.ToLower()).ToArray();

        return new QueryContainerDescriptor<T>()
            .MultiMatch(c => c
                .Type(TextQueryType.Phrase)
                .Fields(f => f.Fields(fields)).Lenient().Query(queryValue));
    }

    public static List<IndexActors> GetSampleData()
    {
        var list = new List<IndexActors>
            {
                new() {Id = "38d61273-8f4e-431c-a0bb-3de2fbbf8757", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1969, 9, 25), Age = 50, TotalMovies = 25, Name = "Catherine Zeta-Jones", Description = "Sua carreira artística começou ainda cedo. Antes dos dez anos de idade, Catherine já cantava e dançava na companhia teatral de uma congregação católica. Aos 15 anos, deixou Swansea e foi para Londres, tentar fazer sucesso nos palcos. Estreou no cinema em 1990, no filme Les mille et une nuits do diretor francês Philippe de Broca. Em 1991, participou da série britânica The Darling Buds of May.[2][3] Em 1992 e 1993 participou do seriado The Young Indiana Jones Chronicles. Em 1995, estrelou no papel-título da minissérie Catherine the Great, sobre a imperatriz russa Catarina II."},
                new() {Id = "6d288857-34e4-417f-8a22-e0c7575972b5", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1948, 12, 27), Age = 71, TotalMovies = 44, Name = "Gérard Depardieu", Description = "Com origens humildes, filho de um operário metalúrgico.[1] Antes de ser considerado o presente francês para o cinema, ou ainda, o Robert De Niro francês, o terceiro dos seis filhos de um pobre operário abandonou a escola aos 12 anos, fugiu de casa e viveu com prostitutas. A vida delinquente e vândala de Depardieu se estendeu até os seus 16 anos de idade, quando foi incentivado por uma assistente social a sair das ruas e investir numa carreira artística. Desde então, começou a marcar presença nos palcos de teatros parisienses, e daí a começar a participar de filmes foi mera questão de tempo. Sua carreira de ator, que já conta com um total de 140 filmes. Ao optar pelos palcos, Depardieu transformou-se no ator carismático que, hoje, é famoso em todo o mundo."},
                new() {Id = "9dc521a4-3486-4c6c-b769-8bdafcc4563d", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1976, 1, 13), Age = 44, TotalMovies = 34, Name = "Michael Peña", Description = "Peña nasceu em Chicago, Illinois, onde seu pai trabalhava em uma fábrica de botões e sua mãe era uma assistente social.[1][2] Os pais de Peña, imigrantes provenientes do México, foram originalmente agricultores.[3][4] Peña frequentou a Hubbard High School em Chicago.[5] Peña e sua esposa, Brie Shaffer, tiveram seu primeiro filho em setembro de 2008, um menino chamado Roman.[6]"},
                new() {Id = "b9044bf7-6917-4521-9b1b-f737a05965f9", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1944, 9, 25),Age = 75, TotalMovies = 52, Name = "Michael Douglas", Description = "Michael Douglas graduou-se em 1968 na Choate School University de Santa Barbara, em American Place Theatre. Ficou conhecido do público ao estrelar para a televisão o seriado The Streets of San Francisco (ao lado do veterano ator Karl Malden). Mas ganharia renome nos meios cinematográficos quando produziu One Flew Over the Cuckoo's Nest."},
                new() {Id = "abc6caf0-6c60-4e9e-93d2-2f53fbe941e2", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1956, 7, 9),Age = 63, TotalMovies = 41, Name = "Tom Hanks", Description = "Thomas Jeffrey Hanks, mais conhecido como Tom Hanks[1] (Concord, 9 de julho de 1956),[2] é um premiado ator, produtor, dublador, roteirista e diretor norte-americano. Destacou-se em diversos filmes de sucesso, como: Forrest Gump, Apollo 13, That Thing You Do!, The Green Mile, The Terminal, Inferno, Saving Private Ryan, You've Got Mail, Sleepless In Seattle. Charlie Wilson's War, Catch Me If You Can, Cast Away, A League Of Their Own, The Da Vinci Code, Captain Phillips, Angels & Demons, Splash, Big, Road To Perdition, Philadelphia e como a voz do personagem Woody na série de filmes de animação Toy Story e também pelas vozes em The Polar Express." },
                new() {Id = "7b0636fd-42ca-4435-a9ff-bebef6a40fda", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1933, 3, 14),Age = 87, TotalMovies = 69, Name = "Michael Caine", Description = "Michael Caine serviu o Exército Britânico na Guerra da Coreia, nos anos cinquenta. Estreou no cinema com o filme A Hill in Korea, de 1956, no qual interpreta um cabo do exército britânico. Seu primeiro papel de destaque no cinema foi no filme Zulu, de 1964, dirigido por Cy Endfield. Caine ficou popular em uma série de filmes realizados nos anos sessenta, no auge da Guerra Fria, nos quais ele interpretou um espião inteligente e racional chamado Harry Palmer." },
                new() {Id = "b7666135-d20b-4a52-98f2-87023d4c2bdb", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1974, 1, 30),Age = 46, TotalMovies = 42, Name = "Christian Bale", Description = "Christian Bale, para cada dez atores mirins que, quando adultos, não conseguem continuar sua carreira, existe um Christian Bale. O menino escolhido pessoalmente por Steven Spielberg para estrelar Império do Sol cresceu e se tornou um dos talentos mais respeitados e um dos homens mais desejados de Hollywood.[carece de fontes]" },
                new() {Id = "6b0b2f52-ee2d-4b90-9dcc-c303d1b9830c", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1943, 8, 17), Age = 76,TotalMovies = 56, Name = "Robert De Niro", Description = "O primeiro filme em que Robert De Niro participou foi a produção francesa de 1965 Trois Chambres à Manhattan, de Marcel Carné. Niro fazia um papel secundário, como cliente de um restaurante.[4] Depois participou em muitos outros filmes, com papéis maiores e menores, mas nenhum deles com grande sucesso, até que atingiu a popularidade com o seu papel em Bang the Drum Slowly (br: A Última Batalha de um Jogador), em 1973. "},
                new() {Id = "2eda34e3-3cdb-47b7-ac56-c5946d7d695a", RegistrationDate = DateTime.Now, BirthDate = new DateTime(1940, 4, 25),Age = 80, TotalMovies = 78, Name = "Al Pacino", Description = "Al Pacino nasceu em Nova York (East Harlem), filho dos Ítalo-americanos Salvatore Pacino e Rose, que se divorciaram quando tinha dois anos.[1] Sua mãe mudou-se para próximo ao Zoológico do Bronx para morar com seus pais, Kate e James Gerardi, que, por coincidência, tinham vindo de uma cidade na Sicília chamada Corleone.[2] Seu pai que era de San Fratello na Província de Messina, mudou-se para Covina, Califórnia, onde trabalhou como vendedor de seguros e gerente/proprietário de restaurante.[1]"}
            };
        return list;
    }

    public static double ObterBucketAggregationDouble(AggregateDictionary agg, string bucket)
    {
        return agg.BucketScript(bucket).Value.HasValue ? agg.BucketScript(bucket).Value.Value : 0;
    }
}