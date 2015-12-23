// Created by: egr
// Created at: 01.05.2015
// © 2015 Alexander Egorov

using System;
using System.Runtime.CompilerServices;

namespace qif2json.parser
{
    public static class Extensions
    {
        /// <summary>
        ///     With monad that defines failure result
        /// </summary>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="input">Input instance</param>
        /// <param name="evaluator">Evaluation function</param>
        /// <param name="failure">Failure result</param>
        /// <returns>if input null returns null evaluator result otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult Return<TInput, TResult>(this TInput input, Func<TInput, TResult> evaluator, TResult failure)
            where TInput : class
        {
            return input == null ? failure : evaluator(input);
        }

        /// <summary>
        ///     Do Monad that runs action if input is not null
        /// </summary>
        /// <typeparam name="TInput">Input type</typeparam>
        /// <param name="input">Input instance</param>
        /// <param name="action">Action to run if input is not null</param>
        /// <returns>if input null returns null otherwise runs action and returns input</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInput Do<TInput>(this TInput input, Action<TInput> action)
            where TInput : class
        {
            if (input == null)
            {
                return null;
            }
            action(input);
            return input;
        }
    }
}