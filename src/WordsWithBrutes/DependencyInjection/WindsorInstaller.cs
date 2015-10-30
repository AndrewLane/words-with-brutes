using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WordsWithBrutes.Components;
using WordsWithBrutes.Components.Impl;

namespace WordsWithBrutes.DependencyInjection
{
    /// <summary>
    /// Helper class to register all our components in the container
    /// </summary>
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //register all our components here
            container.Register(Component.For<ISolveChallenges>().ImplementedBy<SolveChallenges>());
            container.Register(Component.For<IDetermineIfAStringIsAPlayableWord>().ImplementedBy<DetermineIfAStringIsAPlayableWord>());
            container.Register(Component.For<IDeterminePotentialPlayLocationsForNextWord>().ImplementedBy<DeterminePotentialPlayLocationsForNextWord>());
            container.Register(Component.For<IDetermineTheTotalPointsAndWordsCreatedByAPlay>().ImplementedBy<DetermineTheTotalPointsAndWordsCreatedByAPlay>());
            container.Register(Component.For<IGenerateWordsToTryForPotentialPlayLocation>().ImplementedBy<GenerateWordsToTryForPotentialPlayLocation>());
            container.Register(Component.For<IConvertGameStateToHumanReadableStrings>().ImplementedBy<ConvertGameStateToHumanReadableStrings>());
            container.Register(Component.For<ITransformGameStateIntoTwoDimensionalArray>().ImplementedBy<TransformGameStateIntoTwoDimensionalArray>());
            container.Register(Component.For<IGenerateStringPermutations>().ImplementedBy<GenerateStringPermutations>());
            container.Register(Component.For<IConvertStringToWordsWithFriendsChallenge>().ImplementedBy<ConvertStringToWordsWithFriendsChallenge>());
            container.Register(Component.For<IPopulateStandardWordsWithFriendsChallenge>().ImplementedBy<PopulateStandardWordsWithFriendsChallenge>());
        }
    }
}
