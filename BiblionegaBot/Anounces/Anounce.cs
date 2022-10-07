using System;

namespace BiblionegaBot.Anounces
{
    public class Anounce
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
        public AnounceCategory Category { get; set; }
    }
}
