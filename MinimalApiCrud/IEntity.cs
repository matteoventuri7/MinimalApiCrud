﻿namespace MinimalApiCrud
{
    public interface IEntity<T>
    {
        public T Id { get; }
    }
}