using AutoMapper;
using BlazorFaceRecog.Client.Components.FaceCard;
using BlazorFaceRecog.Shared;

namespace BlazorFaceRecog.Client.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SavedFaceModel, FaceCardViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => src.Thumbnail))
            .ForMember(dest => dest.State, opt => opt.MapFrom(_ => CardState.Saved));

        CreateMap<FaceCardViewModel, TrainFaceModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}